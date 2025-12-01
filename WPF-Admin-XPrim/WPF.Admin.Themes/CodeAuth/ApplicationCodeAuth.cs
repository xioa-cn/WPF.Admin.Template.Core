using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using HandyControl.Controls;
using WPF.Admin.Models.Models;
using WPF.Admin.Models.Utils;
using WPF.Admin.Service.Logger;
using WPF.Admin.Service.Services;
using WPF.Admin.Service.Utils;
using WPF.Admin.Themes.Converter;
using WPF.Admin.Themes.Helper;

namespace WPF.Admin.Themes.CodeAuth
{
    public static class ApplicationCodeAuth
    {
        public static DateTime StartTime
        {
            get { return ApplicationAuthModule.StartTime; }
        } // 体验时间开始时间

        public static bool AuthTaskFlag
        {
            get => ApplicationAuthModule.AuthTaskFlag;
            set { ApplicationAuthModule.AuthTaskFlag = value; }
        } // 是否开启授权任务

        /// <summary>
        /// 授权标志 True为授权失败
        /// </summary>
        public static bool AuthFlag
        {
            get
            {
                if (AuthTaskFlag)
                    return (DateTime.Now - StartTime).TotalHours >= ApplicationAuthModule._Interval ? true : false;
                return false;
            }
        }

        private static async Task AuthMethod()
        {
            AuthTaskFlag = true;
            var batFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory
                , "WPFAdmin.auc.bat");

            // var readContent = await System.IO.File.ReadAllTextAsync(batFile);
            //
            // var readList = readContent.Split('\n');
            // var str = new StringBuilder();
            // foreach (var item in readList)
            // {
            //     if (item.Contains("echo !userInput! >"))
            //     {
            //         str.Append($"echo !userInput! > {AppDataPath.GetLocalFilePath("authcode.txt")}" + "\n");
            //     }
            //     else
            //     {
            //         str.Append(item + "\n");
            //     }
            // }


            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = batFile;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            using Process process = new Process();
            process.StartInfo = startInfo;
            try
            {
                // 启动进程
                process.Start();
                process.WaitForExit();
                int exitCode = process.ExitCode;
                Console.WriteLine($"BAT文件执行完毕，退出代码: {exitCode}");
                await Task.Delay(1000);
                var createFile = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory
                    , "authcode.txt");
                var authCode = await System.IO.File.ReadAllTextAsync(createFile
                );
                await System.IO.File.WriteAllTextAsync(AppDataPath.GetLocalFilePath("authcode.txt"),
                    authCode);

                System.IO.File.Delete(createFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"执行BAT文件时出错: {ex.Message}");
            }
            finally
            {
                Environment.Exit(0);
            }
        }


        public static void AuthTask(bool isOpen = false)
        {
            AuthTaskFlag = true;
            Task.Run(async () =>
            {
                while (true)
                {
                    if ((DateTime.Now - StartTime).TotalHours >= ApplicationAuthModule._Interval - 0.5 || isOpen)
                    {
                        Thread.Sleep(5000);
                        if (!isOpen)
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                Growl.ErrorGlobal("体验时间限制到期！！ 请申请正式版软件 或 添加激活码");
                            });
                        await AuthMethod();
                    }

                    await Task.Delay(1800000);
                }
            });
        }

        private static string StartupHelper(string authPathFile)
        {
            if (!System.IO.File.Exists(authPathFile) && ApplicationAuthModule.AuthOutTime > DateTime.Now)
            {
                AppAuthor.Version = $"Authorization date {AppSettings.Default.AuthOutTime}";
                throw new Exception("Authorization date");
            }

            var authcode = System.IO.File.ReadAllText(
                authPathFile).Replace("\n", "").Replace("\r", "").Replace(" ", "");

            // var nor = TextCodeHelper.Encrypt();

            var dcode = TextCodeHelper.Decrypt(authcode);

            var timeCode = TextCodeHelper.Decrypt(authcode, "DT:----");


            if (timeCode.Contains(".XA"))
            {
                var time = timeCode.Replace(".XA", "");
                var time2 = DateTime.ParseExact(time, "yyyyMMdd", CultureInfo.InvariantCulture);

                if (time2 > DateTime.Now)
                {
                    var timeSaveString = time2.ToString("yyyy-MM-dd");

                    AuthTaskFlag = false;
                    AppSettings.Default.AuthOutTime = timeSaveString;

                    LargeTextEncryptor.EncryptLargeText(
                        JsonSerializer.Serialize(AppSettings.Default, SerializeHelper._options),
                        ApplicationConfigConst.SettingEncryptorJsonFile);
                    System.IO.File.Delete(authPathFile);
                    AppAuthor.Version = $"Authorization date {timeSaveString}";
                    return authcode;
                }
            }


            var auth = nasduabwduadawdb(dcode);

            if (auth == ApplicationConfigConst.Code)
            {
                AuthTaskFlag = false;
                AppAuthor.Version = "Authorization successful";
            }
            else
            {
                if (ApplicationAuthModule.AuthOutTime < DateTime.Now)
                {
                    AuthTask();
                }
                else
                {
                    AppAuthor.Version = $"Authorization date {AppSettings.Default.AuthOutTime}";
                }
            }

            return authcode;
        }

        public static void Startup()
        {
            string authcode = "string.Empty";
            try
            {
                var authPathFile = AppDataPath.GetLocalFilePath("authcode.txt");
                var timeError = SystemTimeHelper.GetHistoryTime();

                if (!timeError)
                {
                    if (!System.IO.File.Exists(authPathFile))
                    {
                        Growl.ErrorGlobal("系统时间可能被篡改！ 软件授权加密被开启");
                        AuthTask(true);
                    }

                    authcode = System.IO.File.ReadAllText(
                        authPathFile).Replace("\n", "").Replace("\r", "").Replace(" ", "");

                    var dcode = TextCodeHelper.Decrypt(authcode);
                    var auth = nasduabwduadawdb(dcode);

                    if (auth == ApplicationConfigConst.Code)
                    {
                        AuthTaskFlag = false;
                        AppAuthor.Version = "Authorization successful";
                    }
                    else
                    {
                        Growl.ErrorGlobal("系统时间可能被篡改！ 软件授权加密被开启");
                        AuthTask(true);
                    }
                }

                //System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "authcode.txt");

                if (AppSettings.Default is not null &&
                    AppSettings.Default.ApplicationVersions == ApplicationVersions.NoAuthorization)
                {
                    authPathFile = TextCodeHelper.NoAuthorizationFile;
                    if (ApplicationAuthModule.DllCreateTime.IsOnTimeDay(DateTime.Now))
                    {
                        TextCodeHelper.NoAuthorizationRequired();
                    }

                    AppAuthor.Version = "Free version";
                }

                if (!System.IO.File.Exists(authPathFile) && ApplicationAuthModule.AuthOutTime < DateTime.Now)
                {
                    var message = "授权文件不存在，请联系管理员获取授权码。";
                    Growl.ErrorGlobal(message);
                    throw new Exception(message);
                }

                if (ListenApplicationVersions.NormalVersion != ApplicationVersions.NoAuthorization)
                {
                    authcode = StartupHelper(authPathFile);
                }
            }
            catch (Exception ex)
            {
                XLogGlobal.Logger?.LogError(ex.Message);

                AuthTask();
            }

            try
            {
                //ReflectionMethods.getMethod<HslCommunication.Authorization>("nasduabwduadawdb");

                Console.WriteLine("Starting method replacement...");


                var result =
                    HslCommunication.Authorization.SetAuthorizationCode("fe49cdb6-b388-4c05-9b66-0e3f1ad3627f");

#if DEBUG
                Console.WriteLine($"Method call result: {result}");

                if (result)
                {
                    Debug.WriteLine("GOD: Here you go!!!");
                }
#else
            if (result)
            {
                Debug.WriteLine("GOD: Here you go!!!");
            }


#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }


        public static string nasduabwduadawdb(string miawdiawduasdhasd)
        {
            StringBuilder stringBuilder = new StringBuilder();
            MD5 mD = MD5.Create();
            byte[] array = mD.ComputeHash(Encoding.Unicode.GetBytes(miawdiawduasdhasd));
            mD.Clear();
            foreach (var t in array)
            {
                stringBuilder.Append((255 - t).ToString("X2"));
            }

            return stringBuilder.ToString();
        }
    }
}