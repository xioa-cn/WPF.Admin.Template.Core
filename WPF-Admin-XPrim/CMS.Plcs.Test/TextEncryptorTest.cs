using WPF.Admin.Service.Utils;

namespace CMS.Plcs.Test
{
    public class TextEncryptorTest
    {
        [Fact]
        public void EncryptDecryptTest()
        {
            var text = "Hello World";
            var key = "mykey";

            var filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils.oa");
            
            // Act
             LargeTextEncryptor.EncryptLargeText(text,filePath);
            var decryptedText = LargeTextEncryptor.DecryptLargeText(filePath);

            // Assert
            Assert.Equal(text, decryptedText);
        }
    }
}