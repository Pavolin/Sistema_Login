using System;
using Utils.Enums;

namespace Utils.Interfaces
{
    public interface ITracer
    {
        /// <summary> 
        /// funcoes usadas no services.trancer
        /// </summary>
        void WriteLine(string line, TraceLevel level);
        void WriteLine(Exception ex);
    }
}
