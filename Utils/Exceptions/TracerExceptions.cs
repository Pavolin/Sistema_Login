using System;

namespace Utils.Exceptions
{
    public class TracerExceptions : ApplicationException
    {
        #region Constructor Methods
        public TracerExceptions(string message)
            : base(message)
        {

        }
        #endregion
    }
}
