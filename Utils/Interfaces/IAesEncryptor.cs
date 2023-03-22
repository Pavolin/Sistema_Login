namespace Utils.Interfaces
{
    public interface IAesEncryptor
    {
        /// <summary> 
        /// funcoes usadas no services.AesEncryptor
        /// </summary>
        string Encrypt(string clearText);

        string Decrypt(string cipherText);
    }
}
