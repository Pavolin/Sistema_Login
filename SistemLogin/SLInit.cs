using API.Services;
using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceProcess;
using Utils.Enums;
using Utils.Services;
using Integration.Interfaces;
using Integration.Services;
using API.Routes;

namespace SistemaLogin
{
    public partial class SLInit : ServiceBase
    {
        #region Attributes
        /// <summary> 
        /// area das referencias ao atributos usados nessa classe
        /// </summary>
        private Tracer _tracer;
        private Configs _configs = new Configs($"{Assembly.GetExecutingAssembly().Location}.config");
        private WebServiceHost _serviceHost;
        private IIntegration _integration;
        private AesEncryptor _encripitador;
        #endregion

        #region Initialize Component
        /// <summary> 
        /// Inicializador dos componentes do serviço
        /// </summary>
        public SLInit()
        {
            InitializeComponent();
        }
        #endregion

        #region Debug Method
        /// <summary> 
        /// funcao usada ao executar o serviço em debug, chamando o OnStart do serviço
        /// </summary>
#if DEBUG
        public void StartDebug(string[] args)
        {
            OnStart(args);
        }
#endif
        #endregion

        #region Check Configuration Method
        /// <summary> 
        /// Funcao para realizar todas as configurações customizadas
        /// Configuracao do roteamento da API e endpoint
        /// </summary>
        private bool CheckConfigurations()
        {
            #region Integration config
            try
            {
                string serverName = this._configs.GetParameter("DBSettings", "serverName", false);
                string dataBaseName = this._configs.GetParameter("DBSettings", "dataBaseName", false);
                string userName = this._configs.GetParameter("DBSettings", "userName", false);
                string password = this._configs.GetParameter("DBSettings", "password", false);
                bool integratedSecurity = Convert.ToBoolean(this._configs.GetParameter("DBSettings", "integratedSecurity", false));

                this._integration = new Funcoes(this._tracer, serverName, dataBaseName, userName, password, integratedSecurity, this._encripitador);

                this._tracer.WriteLine("Verificacao e atribuicao de valores[OK]", TraceLevel.Info);
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine("Verificacao e atribuicao de valores[Erro]", TraceLevel.Info);
                throw ex;
            }
            #endregion

            #region APIConfig
            bool result = true;
            this._tracer.WriteLine($"{this.ServiceName}.CheckConfigurations()", TraceLevel.Debug);
            this._tracer.WriteLine($"Verificando configuracoes do servico", TraceLevel.Debug);

            try
            {
                #region Web Server
                try
                {
                    string baseURLStr;
                    baseURLStr = this._configs.GetParameter("webServerSettings", "baseAddress", false);
                    if (baseURLStr[baseURLStr.Length - 1] == '/') baseURLStr.Remove(baseURLStr.Length - 1);

                    string bearerToken = this._configs.GetParameter("webServerSettings", "bearerToken");
                    bearerToken = string.IsNullOrEmpty(bearerToken) ? bearerToken : $"Bearer {bearerToken}";

                    Uri uri = new Uri(baseURLStr);
                    WebHttpBinding binding = new WebHttpBinding();
                    WebServer webServer = new WebServer(this._tracer, bearerToken, this._integration, baseURLStr);

                    this._serviceHost = new WebServiceHost(webServer, uri);
                    this._serviceHost.AddServiceEndpoint(typeof(IGeneratorToken), binding, "gerarToken");
                    this._serviceHost.AddServiceEndpoint(typeof(ICreateTable), binding, "criarTabela");
                    this._serviceHost.AddServiceEndpoint(typeof(IRegisterUser), binding, "registrarUsuario");
                    this._serviceHost.AddServiceEndpoint(typeof(ILogin), binding, "login");
                    this._serviceHost.AddServiceEndpoint(typeof(IChangePermission), binding, "mudarPermissao");
                    this._serviceHost.AddServiceEndpoint(typeof(IDeleteAccount), binding, "deletarConta");
                    this._serviceHost.AddServiceEndpoint(typeof(IBlockAccount), binding, "bloquearConta");
                    this._serviceHost.AddServiceEndpoint(typeof(IUnlockAccount), binding, "desbloquearConta");
                    this._serviceHost.AddServiceEndpoint(typeof(IActiveAccount), binding, "ativarConta");
                    this._serviceHost.AddServiceEndpoint(typeof(IRecoverPassword), binding, "recuperarSenha");
                    this._serviceHost.AddServiceEndpoint(typeof(IChangePassword), binding, "alterarSenha");
                    this._serviceHost.Open();

                    this._tracer.WriteLine($"Endereco da API: {uri.AbsoluteUri}", TraceLevel.Debug);

                    this._tracer.WriteLine("Acesso a API[OK]", TraceLevel.Info);
                }
                catch (Exception ex)
                {
                    this._tracer.WriteLine("Acesso a API[Erro]", TraceLevel.Error);
                    throw ex;
                }
                #endregion

               
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine(ex);
                result = false;
            }

            return result;
            #endregion
        }
        #endregion

        #region Start e Stop do servico
        /// <summary> 
        /// funcoes usadas ao serviço inciar e finalizar
        /// na funcao de start é configurado os LOGs e chamado a funcao de configuracao da API
        /// </summary>
        protected override void OnStart(string[] args)
        {
            #region Tracer Initializer
            try
            {
                this._tracer = new Tracer(this._configs.GetParameter("tracerSettings", "tracePath", false))
                {
                    Enabled = this._configs.GetParameter("tracerSettings", "enable").ToLower() == "true",
                    DetailedException = this._configs.GetParameter("tracerSettings", "detailedException").ToLower() == "true"
                };
            }
            catch
            {
                this.Stop();
            }
            try
            {
                this._tracer.SetTracerLevel(this._configs.GetParameter("tracerSettings", "traceLevel", false));
            }
            catch
            {
                this._tracer.SetTracerLevel("Full");
            }
            try
            {
                string maxSize = this._configs.GetParameter("tracerSettings", "maxSize");
                if (maxSize != null) _tracer.SetMaxSize(maxSize);
            }
            catch (Exception ex)
            {
                _tracer.WriteLine(ex.Message, TraceLevel.Error);
            }
            #endregion

            #region Tracer AesEncryptor
            try
            {
                this._encripitador = new AesEncryptor();
            }
            catch(Exception ex)
            {
                _tracer.WriteLine(ex.Message, TraceLevel.Error);
                this.Stop();
            }
            #endregion

            this._tracer.WriteLine($"{this.ServiceName}.OnStart()", TraceLevel.Debug);
            this._tracer.WriteLine($"Iniciando o servico \"{this.ServiceName}\"", TraceLevel.Info);

            if (!this.CheckConfigurations())
            {
                this._tracer.WriteLine("Problemas nas configuracoes do servico", TraceLevel.Info);
                this._tracer.WriteLine("Parando o servico...", TraceLevel.Info);
                this.Stop();
                while (true) ;
            }

            this._tracer.WriteLine($"Servico \"{this.ServiceName}\" iniciado com sucesso", TraceLevel.Info);
        }

        protected override void OnStop()
        {
            this._tracer.WriteLine($"{this.ServiceName}.OnStop()", TraceLevel.Debug);
            try
            {
                if (this._serviceHost.State == CommunicationState.Opened)
                    this._serviceHost.Close();

            }
            catch (Exception ex)
            {
                this._tracer.WriteLine(ex);
            }
            finally
            {
                this._tracer.WriteLine($"Servico \"{this.ServiceName}\" finalizado", TraceLevel.Info);
            }
        }
        #endregion
    }
}
