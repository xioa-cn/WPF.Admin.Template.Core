using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using WPF.Admin.Models.Utils;

namespace WPF.Admin.Service.Utils
{
    public class LargeTextEncryptor
    {
        private static readonly byte[] _salt = Encoding.UTF8.GetBytes(ApplicationConfigConst.TextSaltValue);

        /// <summary>
        /// 加密超长文本并以字符形式（Base64）保存到文件
        /// </summary>
        public static void EncryptLargeText(string text, string filePath, string? password = null)
        {
            if (password == null)
            {
                password = ApplicationConfigConst.LargeTextPassword;
            }

            // 输入验证
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("待加密文本不能为空", nameof(text));
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("文件路径不能为空", nameof(filePath));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("密码不能为空", nameof(password));

            using (Aes aes = Aes.Create())
            {
                // 生成密钥和IV
                using (var keyDeriver = new Rfc2898DeriveBytes(
                           password,
                           _salt,
                           100000,
                           HashAlgorithmName.SHA256))
                {
                    aes.Key = keyDeriver.GetBytes(32); // 256位密钥
                    aes.IV = keyDeriver.GetBytes(16);  // 128位IV
                }

                // 加密过程：先将文本加密为字节，再转换为Base64字符
                byte[] encryptedBytes;
                using (var memoryStream = new MemoryStream())
                {
                    // 先写入IV（后续会和加密数据一起转为Base64）
                    memoryStream.Write(aes.IV, 0, aes.IV.Length);

                    // 加密文本
                    using (var cryptoStream = new CryptoStream(
                               memoryStream,
                               aes.CreateEncryptor(),
                               CryptoStreamMode.Write))
                    {
                        byte[] textBytes = Encoding.UTF8.GetBytes(text);
                        cryptoStream.Write(textBytes, 0, textBytes.Length);
                    }

                    // 获取IV+加密数据的完整字节数组
                    encryptedBytes = memoryStream.ToArray();
                }

                // 关键：将字节数组转换为Base64字符串（纯字符，可安全存储为文本）
                string encryptedBase64 = Convert.ToBase64String(encryptedBytes);

                // 以字符形式写入文件（UTF8编码，纯文本）
                File.WriteAllText(filePath, encryptedBase64, new UTF8Encoding(false));

                // 验证文件内容完整性
                if (string.IsNullOrEmpty(File.ReadAllText(filePath, new UTF8Encoding(false))))
                    throw new InvalidOperationException("加密文件写入失败");
            }
        }

        /// <summary>
        /// 从字符形式（Base64）的文件中解密文本
        /// </summary>
        public static string DecryptLargeText(string filePath, string? password = null)
        {
            if (password == null)
            {
                password = ApplicationConfigConst.LargeTextPassword;
            }

            // 输入验证
            if (!File.Exists(filePath))
                throw new FileNotFoundException("加密文件不存在", filePath);
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("密码不能为空", nameof(password));

            // 读取文件中的Base64字符（纯文本）
            string encryptedBase64 = File.ReadAllText(filePath, new UTF8Encoding(false));
            if (string.IsNullOrEmpty(encryptedBase64))
                throw new InvalidDataException("加密文件内容为空");

            // 将Base64字符串转回字节数组
            byte[] encryptedBytes;
            try
            {
                encryptedBytes = Convert.FromBase64String(encryptedBase64);
            }
            catch (FormatException ex)
            {
                throw new InvalidDataException("加密文件格式错误，不是有效的Base64字符串", ex);
            }

            // 验证总字节长度是否足够（至少包含16字节IV）
            if (encryptedBytes.Length < 16)
                throw new InvalidDataException("加密文件损坏：数据长度不足");

            using (Aes aes = Aes.Create())
            {
                // 提取IV（前16字节）
                byte[] iv = new byte[16];
                Array.Copy(encryptedBytes, 0, iv, 0, 16);
                aes.IV = iv;

                // 生成密钥
                using (var keyDeriver = new Rfc2898DeriveBytes(
                           password,
                           _salt,
                           100000,
                           HashAlgorithmName.SHA256))
                {
                    aes.Key = keyDeriver.GetBytes(32);
                }

                // 解密剩余的加密数据（跳过IV的16字节）
                using (var memoryStream = new MemoryStream(encryptedBytes, 16, encryptedBytes.Length - 16))
                using (var cryptoStream = new CryptoStream(
                           memoryStream,
                           aes.CreateDecryptor(),
                           CryptoStreamMode.Read))
                using (var resultStream = new MemoryStream())
                {
                    byte[] buffer = new byte[8192];
                    int bytesRead;
                    while ((bytesRead = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        resultStream.Write(buffer, 0, bytesRead);
                    }

                    // 转换为原始文本
                    return Encoding.UTF8.GetString(resultStream.ToArray());
                }
            }
        }
    }
}