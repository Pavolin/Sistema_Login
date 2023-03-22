using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using Utils.Exceptions;
using Utils.Interfaces;

namespace Utils.Services
{
    public class AesEncryptor : IAesEncryptor
    {
        #region Atributtes of the class
        /// <summary> 
        /// area das referencias ao atributos usados nessa classe
        /// </summary>
        private const string _password = "sistemalogin";
        private const string _saltPhrase = "## API para sistema de login, REST ##";
        #endregion

        #region Constructor Methods
        /// <summary> 
        /// area dos construtores da classe
        /// </summary>
        public AesEncryptor()
        {

        }
        #endregion

        #region Methods
        /// <summary> 
        /// area dos metodos usados em outras classes
        /// </summary>
        public string Encrypt(string clearText)
        {
            string EncryptionKey = _password;
            byte[] salt = Encoding.ASCII.GetBytes(_saltPhrase);
            byte[] clearBytes = Encoding.UTF8.GetBytes(clearText);
            string cipherText = "";
            if (clearText == String.Empty) throw new AesEncryptorExceptions("clearText was Empty");
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, salt);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                encryptor.Padding = PaddingMode.PKCS7;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    cipherText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return cipherText;
        }

        public string Decrypt(string cipherText)
        {
            string EncryptionKey = _password;
            byte[] salt = Encoding.ASCII.GetBytes(_saltPhrase);
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            string clearText = "";
            if (cipherText == String.Empty) throw new AesEncryptorExceptions("cipherText was Empty");
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, salt);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                encryptor.Padding = PaddingMode.PKCS7;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    clearText = Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            return clearText;
        }
        #endregion
    }
}
