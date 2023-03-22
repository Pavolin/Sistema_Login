using System;

namespace Utils.Exceptions
{
    public class ConfigsExceptions : ApplicationException
    {
        #region Constructor Methods
        public ConfigsExceptions(string message)
            : base(message)
        {

        }
        #endregion
    }
}
