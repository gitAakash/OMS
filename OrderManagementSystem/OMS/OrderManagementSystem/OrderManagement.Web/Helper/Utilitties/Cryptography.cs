using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace OrderManagement.Web.Helper.Utilitties
{
    public class Cryptography
    {
        #region Fields

        private static byte[] key = { };
        private static byte[] IV = { 56, 34, 101, 50, 5, 14, 45, 9 };
        private static string stringKey = "res#ft!vbvp04";

        #endregion
        #region Public Methods

        public static string EncryptOld(string text)
        {
            try
            {
                key = Encoding.UTF8.GetBytes(stringKey.Substring(0, 8));
                var des = new DESCryptoServiceProvider();
                Byte[] byteArray = Encoding.UTF8.GetBytes(text);
                var memoryStream = new MemoryStream();
                var cryptoStream = new CryptoStream(memoryStream,
                des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cryptoStream.Write(byteArray, 0, byteArray.Length);
                cryptoStream.FlushFinalBlock();

                return Convert.ToBase64String(memoryStream.ToArray());
            }
            catch (Exception ex)
            {
                
            }
            return string.Empty;
        }

        public static string DecryptOld(string text)
        {
            try
            {
                key = Encoding.UTF8.GetBytes(stringKey.Substring(0, 8));
                var des = new DESCryptoServiceProvider();
                Byte[] byteArray = Convert.FromBase64String(text);
                var memoryStream = new MemoryStream();
                var cryptoStream = new CryptoStream(memoryStream,
                    des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cryptoStream.Write(byteArray, 0, byteArray.Length);
                cryptoStream.FlushFinalBlock();

                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            catch (Exception ex)
            {
               
            }
            return string.Empty;
        }

        /// <summary>
        /// AES Algorithm Encryption and Decryption functions
        /// use of AES encryption algorithm wherein I am using a Symmetric (Same) key for encryption and decryption process.
        /// Firstly the original text i.e. clear text is converted into bytes and then for the AES algorithm to perform encryption, we need to generate Key and IV using the derived bytes and the symmetric key.
        ///Using MemoryStream and CryptoStream the clear text is encrypted and written to byte array and finally the byte array is converted to Base64String and returned which is the final outcome i.e. the corresponding encrypted text.
        /// </summary>
        /// <param name="clearText"></param>
        /// <returns></returns>

        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "OMSPWD";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }


        /// <summary>
        /// Decryption
        ///Firstly the encrypted text i.e. cipher text is converted into bytes and then similar to the encryption process here too we will generate Key 
        ///and IV using the derived bytes and the symmetric key.
        ///Using MemoryStream and CryptoStream the cipher text is decrypted and written to byte array and finally the byte array is converted to Base64String and returned,
        ///which is the decrypted original text.
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "OMSPWD";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }


        #endregion
    }
}