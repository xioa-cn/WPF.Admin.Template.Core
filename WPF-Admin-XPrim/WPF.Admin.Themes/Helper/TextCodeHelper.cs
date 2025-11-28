using System.IO;
using System.Security.Cryptography;
using System.Text;
using WPF.Admin.Models.Models;
using WPF.Admin.Models.Utils;
using WPF.Admin.Service.Logger;
using WPF.Admin.Service.Utils;
using WPF.Admin.Themes.CodeAuth;

namespace WPF.Admin.Themes.Helper {
/**
 * 文本代码助手类，用于处理授权相关的加密和解密操作
 */
    public class TextCodeHelper {
        // 无授权文件路径的静态字段，从应用程序配置常量中获取
        public static string NoAuthorizationFile =
            ApplicationConfigConst.NoAuthorizationFile;

        /**
         * 检查和处理授权需求的方法
         * 如果无授权文件不存在，则尝试从资源中读取授权代码并进行验证
         */
        public static void NoAuthorizationRequired() {
            // 检查无授权文件是否存在，如果存在则直接返回
            if (System.IO.File.Exists(NoAuthorizationFile))
            {
                return;
            }

            // 从应用程序资源中查找授权代码文件流信息
            var sri = ApplicationUtils
                .FindApplicationResourceStreamInfo("WPF.Admin.Themes", "Resources/authcode.dll");

            // 如果找不到资源流信息，则直接返回
            if (sri == null)
            {
                return;
            }

            // 使用流读取器读取资源内容
            using StreamReader reader = new StreamReader(sri.Stream);
            try
            {
                // 读取资源内容并解密
                string result = reader.ReadToEnd();
                var result2 = Decrypt(result);
                // 验证解密后的代码是否与配置中的代码匹配
                if (ApplicationCodeAuth.nasduabwduadawdb(result2) == ApplicationConfigConst.Code)
                {
                    // 如果验证通过，将解密后的代码写入无授权文件
                    System.IO.File.WriteAllText(NoAuthorizationFile, result2);
                }

                // 输出解密结果
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                // 记录异常信息
                XLogGlobal.Logger?.LogError(ex.Message, ex);
            }
        }


        // 加密密钥的静态只读字段，基于应用程序认证模块的创建时间生成
        private static readonly string Key = ApplicationAuthModule.DllCreateTime.TimeYearMonthDayHourString();


        /**
         * 加密方法
         * @param plainText 要加密的明文，如果为空则使用默认值
         * @param key 加密密钥，如果为空则使用默认密钥
         * @return Base64编码的加密字符串
         */
        public static string Encrypt(string? plainText = null, string key = "") {
            // 如果未提供密钥，则使用默认密钥
            if (string.IsNullOrEmpty(key))
            {
                key = Key; 
            }

            // 对密钥进行处理，添加固定后缀
            key += "14332231";

            // 如果未提供明文，则使用默认值
            if (string.IsNullOrEmpty(plainText))
            {
                plainText = ApplicationConfigConst._nnnnnnnnnnnnnn; 
            }

          
            // 创建AES加密实例
            using (var aes = Aes.Create())
            {
                // 生成初始化向量(IV)
                aes.GenerateIV();
                var iv = aes.IV;
                
                // 从密钥派生加密密钥
                var aesKey = DeriveKey(key, aes.KeySize);
                
                byte[] encryptedBytes;
                // 创建加密流并执行加密操作
                using (var encryptor = aes.CreateEncryptor(aesKey, iv))
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    cs.Write(plainBytes, 0, plainBytes.Length);
                    cs.FlushFinalBlock();
                    encryptedBytes = ms.ToArray();
                }

             
                // 合并IV和加密后的字节数组
                var combined = new byte[2 + iv.Length + encryptedBytes.Length];
              
                // 将IV长度写入合并数组的前两个字节
                BitConverter.GetBytes((short)iv.Length).CopyTo(combined, 0);
                // 将IV复制到合并数组
                iv.CopyTo(combined, 2);
                // 将加密后的字节数组复制到合并数组
                encryptedBytes.CopyTo(combined, 2 + iv.Length);

                // 将合并后的字节数组转换为Base64字符串并返回
                return Convert.ToBase64String(combined);
            }
        }

        /**
         * 从密钥派生加密密钥的方法
         * @param key 原始密钥字符串
         * @param keySize 所需的密钥大小（位）
         * @return 派生后的密钥字节数组
         */
        private static byte[] DeriveKey(string key, int keySize) {
            // 使用SHA256哈希算法创建密钥派生器
            using (var sha256 = SHA256.Create())
            {
                // 将密钥字符串转换为字节数组并计算哈希值
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                byte[] hash = sha256.ComputeHash(keyBytes);

              
                // 根据所需的密钥大小截取或填充哈希值
                int bytesNeeded = keySize / 8;
                if (hash.Length > bytesNeeded)
                {
                    byte[] result = new byte[bytesNeeded];
                    Array.Copy(hash, result, bytesNeeded);
                    return result;
                }

                return hash;
            }
        }

        /**
         * 解密方法
         * @param cipherText Base64编码的密文字符串
         * @param key 解密密钥，如果为空则使用默认密钥
         * @return 解密后的明文字符串，如果解密失败则返回空字符串
         */
        public static string Decrypt(string cipherText, string key = "") {
            // 如果密文为空，直接返回空字符串
            if (string.IsNullOrEmpty(cipherText))
            {
                return string.Empty;
            }

            try
            {
              
                // 如果未提供密钥，则使用默认密钥
                if (string.IsNullOrEmpty(key))
                {
                    key = Key; 
                }

                // 对密钥进行处理，添加固定后缀
                key += "14332231";
                
                // 将Base64字符串转换为字节数组
                var combined = Convert.FromBase64String(cipherText);
                
                // 从字节数组中提取IV长度
                short ivLength = BitConverter.ToInt16(combined, 0); 
                // 创建IV字节数组
                var iv = new byte[ivLength];
                // 创建加密后的字节数组
                var encryptedBytes = new byte[combined.Length - 2 - ivLength];

                // 从合并数组中复制IV和加密数据
                Array.Copy(combined, 2, iv, 0, ivLength);
                Array.Copy(combined, 2 + ivLength, encryptedBytes, 0, encryptedBytes.Length);
                
                // 创建AES解密实例
                using (var aes = Aes.Create())
                {
                    // 从密钥派生解密密钥
                    var aesKey = DeriveKey(key, aes.KeySize);

                    // 解密
                    using (var decryptor = aes.CreateDecryptor(aesKey, iv))
                    using (var ms = new MemoryStream(encryptedBytes))
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (var sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                XLogGlobal.Logger?.LogError($"解密失败: {ex.Message}");
                return string.Empty;
            }
        }
    }
}