using SistemaLogin;
using System.ServiceProcess;

namespace SistemLogin
{
    static class Program
    {
        /// <summary>
        /// Classe inicalizadora, com os metodos de debug e release, iniciando a API
        /// </summary>
        static void Main(string[] args)
        {
            #region Debug Method
            if (System.Diagnostics.Debugger.IsAttached)
            {
#if (!DEBUG)
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
            {
                new SLInit()
            };
            ServiceBase.Run(ServicesToRun);
#else
                SLInit service = new SLInit();
                service.StartDebug(new string[2]);
                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
#endif
            }
            #endregion

            #region Release Method
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new SLInit()
                };
                ServiceBase.Run(ServicesToRun);
            }
            #endregion
        }
    }
}
