using System;

namespace Utils.Exceptions
{
    public class AesEncryptorExceptions : ApplicationException
    {
        #region Constructor Methods
        public AesEncryptorExceptions(string message)
            : base(message)
        {

        }
        #endregion
    }
}