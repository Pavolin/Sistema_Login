using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;

using API.Routes;
using API.Arguments.CreateTable;

using TraceLevel = Utils.Enums.TraceLevel;
using Utils.Interfaces;
using Integration.Interfaces;
using API.Arguments.RegisterUser;
using System.Text.RegularExpressions;
using API.Arguments.Login;
using API.Arguments.GeneratorToken;
using System.Collections.Generic;
using API.Arguments.ChangePermission;
using API.Arguments.DeleteAccount;
using API.Arguments.BlockAccount;
using API.Arguments.UnlockAccount;
using API.Arguments.ActiveAccount;
using API.Arguments.RecoverPassword;
using API.Arguments.ChangePassword;

namespace API.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = false)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WebServer : ICreateTable, IRegisterUser, ILogin, IGeneratorToken, IChangePermission, IDeleteAccount, IBlockAccount, IUnlockAccount, IActiveAccount, IRecoverPassword, IChangePassword
    {
        #region Attributes
        /// <summary> 
        /// area das referencias ao atributos usados nessa classe
        /// </summary>
        private readonly string _templateLog = @"Request: {0} {1}, User Agent: {2}";
        private readonly string _bearer;
        private readonly string _addr;
        private readonly ITracer _tracer;
        private readonly IIntegration _integration;
        private KeyValuePair<int, string> retorno;

        #endregion

        #region Constructor Methods
        /// <summary> 
        /// area das configuracao dos metodos referenciado na classe SLinit
        /// </summary>
        public WebServer(ITracer tracer, string bearer, IIntegration integration, string addr)
        {
            this._bearer = bearer;
            this._tracer = tracer;
            this._integration = integration;
            this._addr = addr;
        }

        #endregion

        #region SubServices
        /// <summary> 
        /// configuracao de permisoes do roteamento da API e autenticação do token
        /// </summary>
        private void _setCommons()
        {
            WebOperationContext context = WebOperationContext.Current;

            context.OutgoingResponse.Headers.Add("Access-Control-Allow-Headers", "*");
            context.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");
            context.OutgoingResponse.Headers.Add("Access-Control-Allow-Methods", "GET, POST, DELETE, PUT, OPTIONS");

            this._tracer.WriteLine(string.Format(_templateLog, context.IncomingRequest.Method, context.IncomingRequest.UriTemplateMatch.BaseUri.AbsoluteUri, context.IncomingRequest.UserAgent), TraceLevel.Notice);

            if (string.IsNullOrEmpty(this._bearer) && context.IncomingRequest.Headers["Referer"] == $"{this._addr}/gerarToken") 
            {
                this._tracer.WriteLine("primeiro acesso", TraceLevel.Info);
                //continua para o endpoint de geracao de token sem validacao de token
            }
            else if (!string.IsNullOrEmpty(this._bearer) && context.IncomingRequest.Headers["Authorization"] != this._bearer)
            {
                if(context.IncomingRequest.UserAgent != null)
                {
                    this._tracer.WriteLine("Bearer Token invalido.", TraceLevel.Error);
                    throw new WebFaultException(HttpStatusCode.Unauthorized);
                }
                
            }
        }

        #endregion

        #region Services
        /// <summary> 
        /// area das chamadas e configuracoes dos endpoints
        /// </summary>

        #region GeneratorToken
        /// <summary> 
        /// configuracoes do endpoint de criar tabela
        /// </summary>

        public ResponseGeneratorToken GeneratorToken(RequestGeneratorToken request)
        {
            this._setCommons();
            if (string.IsNullOrEmpty(request.token))
            {
                return new ResponseGeneratorToken
                {
                    codeResult = 2,
                    result = "Campo token esta vazio!",
                };
            }
            try
            {
                retorno = this._integration.GeneratorToken(request.token);
                return new ResponseGeneratorToken
                {
                    codeResult = retorno.Key,
                    result = retorno.Value
                };
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine(ex.Message, TraceLevel.Error);
                return new ResponseGeneratorToken
                {
                    codeResult = 2,
                    result = $"Requisição falhou, erro: {ex.Message}",
                };
            }
        }

        #endregion

        #region CreateTable
        /// <summary> 
        /// configuracoes do endpoint de criar tabela
        /// </summary>

        public ResponseCreateTable CreateTable(RequestCreateTable request)
        {
            this._setCommons();
            if (string.IsNullOrEmpty(request.nomeTabela))
            {
                return new ResponseCreateTable
                {
                    codeResult = 2,
                    result = "Campo nomeTabela esta vazio!",
                };
            } 
            try
            {
                retorno = this._integration.CreateTable(request.nomeTabela);
                return new ResponseCreateTable
                {
                    codeResult = retorno.Key,
                    result = retorno.Value
                };
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine(ex.Message, TraceLevel.Error);
                return new ResponseCreateTable
                {
                    codeResult = 2,
                    result = $"Requisição falhou, erro: {ex.Message}",
                };
            }
        }

        #endregion

        #region RegisterUser
        /// <summary> 
        /// configuracoes do endpoint de registrar usuario
        /// </summary>

        public ResponseRegisterUser RegisterUser(RequestRegisterUser request)
        {
            this._setCommons();
            
            if (string.IsNullOrEmpty(request.email))
            {
                return new ResponseRegisterUser
                {
                    codeResult = 2,
                    result = "Campo email esta vazio!",
                };
            }
            else 
            {
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(request.email);
                if (!match.Success)
                {
                    return new ResponseRegisterUser
                    {
                        codeResult = 2,
                        result = "Campo email nao esta no padrao de um email valido!",
                    };
                }
            }

            if (string.IsNullOrEmpty(request.nome))
            {
                return new ResponseRegisterUser
                {
                    codeResult = 2,
                    result = "Campo nome esta vazio!",
                };
            }
            else 
            {
                Regex regex = new Regex(@"^([\w\.\-]+)\.([\w\-]+)$");
                Match match = regex.Match(request.nome);
                if (!match.Success)
                {
                    return new ResponseRegisterUser
                    {
                        codeResult = 2,
                        result = "Campo nome nao esta no padrao primeiroNome.ultimoNome!",
                    };
                }
            }

            if (string.IsNullOrEmpty(request.senha))
            {
                return new ResponseRegisterUser
                {
                    codeResult = 2,
                    result = "Campo senha esta vazio!",
                };
            }
            else
            {
                Regex regex = new Regex(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[$*&@#])[0-9a-zA-Z$*&@#]{8,}$");
                Match match = regex.Match(request.senha);
                if (!match.Success)
                {
                    return new ResponseRegisterUser
                    {
                        codeResult = 2,
                        result = "Campo senha não é forte o suficiente, minimo oito caracteres, uma letra maiuscula, um numero, 1 simbulo entre $*&@# e sem caracteres sequenciais como aa, bb, 44, etc!",
                    };
                }
            }

            if (string.IsNullOrEmpty(request.confirmarSenha))
            {
                return new ResponseRegisterUser
                {
                    codeResult = 2,
                    result = "Campo confirmar senha esta vazio!",
                };
            }
            else if(request.senha != request.confirmarSenha)
            {
                return new ResponseRegisterUser
                {
                    codeResult = 2,
                    result = "Senha não confere!",
                };
            }

            if (string.IsNullOrEmpty(request.telefone))
            {
                return new ResponseRegisterUser
                {
                    codeResult = 2,
                    result = "Campo telefone esta vazio!",
                };
            }

            if (string.IsNullOrEmpty(request.nomeTabela))
            {
                return new ResponseRegisterUser
                {
                    codeResult = 2,
                    result = "Campo nomeTabela esta vazio!",
                };
            }

            try
            {
                retorno = this._integration.RegisterUser(request.email, request.nome, request.senha, request.telefone, request.nomeTabela);
                return new ResponseRegisterUser
                {
                    codeResult = retorno.Key,
                    result = retorno.Value
                };
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine(ex.Message, TraceLevel.Error);
                return new ResponseRegisterUser
                {
                    codeResult = 2,
                    result = $"Requisição falhou, erro: {ex.Message}",
                };
            }
        }
        #endregion

        #region Login
        /// <summary> 
        /// configuracoes do endpoint de login do usuario
        /// </summary>
        public ResponseLogin Login(RequestLogin request)
        {
            this._setCommons();
            if (string.IsNullOrEmpty(request.nomeTabela))
            {
                return new ResponseLogin
                {
                    codeResult = 2,
                    result = "Campo nomeTabela esta vazio!",
                };
            }
            if (string.IsNullOrEmpty(request.email))
            {
                return new ResponseLogin
                {
                    codeResult = 2,
                    result = "Campo email esta vazio!",
                };
            }
            if (string.IsNullOrEmpty(request.senha))
            {
                return new ResponseLogin
                {
                    codeResult = 2,
                    result = "Campo senha esta vazio!",
                };
            }
            try
            {
                retorno = this._integration.Login(request.email, request.senha, request.nomeTabela);
                return new ResponseLogin
                {
                    codeResult = retorno.Key,
                    result = retorno.Value
                };
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine(ex.Message, TraceLevel.Error);
                return new ResponseLogin
                {
                    codeResult = 2,
                    result = $"Requisição falhou, erro: {ex.Message}",
                };
            }
        }
        #endregion

        #region ChangePermission
        /// <summary> 
        /// configuracoes do endpoint de mudar permissao do usuario
        /// </summary>
        public ResponseChangePermission ChangePermission(RequestChangePermission request)
        {
            this._setCommons();
            if (string.IsNullOrEmpty(request.nomeTabela))
            {
                return new ResponseChangePermission
                {
                    codeResult = 2,
                    result = "Campo nomeTabela esta vazio!",
                };
            }
            if (string.IsNullOrEmpty(request.email))
            {
                return new ResponseChangePermission
                {
                    codeResult = 2,
                    result = "Campo email esta vazio!",
                };
            }
            if (string.IsNullOrEmpty(request.permissao))
            {
                return new ResponseChangePermission
                {
                    codeResult = 2,
                    result = "Campo permissao esta vazio!",
                };
            }
            try
            {
                retorno = this._integration.ChangePermission(request.email, request.permissao, request.nomeTabela);
                return new ResponseChangePermission
                {
                    codeResult = retorno.Key,
                    result = retorno.Value
                };
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine(ex.Message, TraceLevel.Error);
                return new ResponseChangePermission
                {
                    codeResult = 2,
                    result = $"Requisição falhou, erro: {ex.Message}",
                };
            }
        }
        #endregion

        #region DeleteAccount
        /// <summary> 
        /// configuracoes do endpoint de mudar permissao do usuario
        /// </summary>
        public ResponseDeleteAccount DeleteAccount(RequestDeleteAccount request)
        {
            this._setCommons();
            if (string.IsNullOrEmpty(request.nomeTabela))
            {
                return new ResponseDeleteAccount
                {
                    codeResult = 2,
                    result = "Campo nomeTabela esta vazio!",
                };
            }
            if (string.IsNullOrEmpty(request.email))
            {
                return new ResponseDeleteAccount
                {
                    codeResult = 2,
                    result = "Campo email esta vazio!",
                };
            }
            try
            {
                retorno = this._integration.DeleteAccount(request.email, request.nomeTabela);
                return new ResponseDeleteAccount
                {
                    codeResult = retorno.Key,
                    result = retorno.Value
                };
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine(ex.Message, TraceLevel.Error);
                return new ResponseDeleteAccount
                {
                    codeResult = 2,
                    result = $"Requisição falhou, erro: {ex.Message}",
                };
            }
        }
        #endregion

        #region BlockAccount
        /// <summary> 
        /// configuracoes do endpoint de bloquear usuario
        /// </summary>
        public ResponseBlockAccount BlockAccount(RequestBlockAccount request)
        {
            this._setCommons();
            if (string.IsNullOrEmpty(request.nomeTabela))
            {
                return new ResponseBlockAccount
                {
                    codeResult = 2,
                    result = "Campo nomeTabela esta vazio!",
                };
            }
            if (string.IsNullOrEmpty(request.email))
            {
                return new ResponseBlockAccount
                {
                    codeResult = 2,
                    result = "Campo email esta vazio!",
                };
            }
            try
            {
                retorno = this._integration.BlockAccount(request.email, request.nomeTabela);
                return new ResponseBlockAccount
                {
                    codeResult = retorno.Key,
                    result = retorno.Value
                };
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine(ex.Message, TraceLevel.Error);
                return new ResponseBlockAccount
                {
                    codeResult = 2,
                    result = $"Requisição falhou, erro: {ex.Message}",
                };
            }
        }
        #endregion

        #region UnlockAccount
        /// <summary> 
        /// configuracoes do endpoint de desbloquear usuario
        /// </summary>
        public ResponseUnlockAccount UnlockAccount(RequestUnlockAccount request)
        {
            this._setCommons();
            if (string.IsNullOrEmpty(request.nomeTabela))
            {
                return new ResponseUnlockAccount
                {
                    codeResult = 2,
                    result = "Campo nomeTabela esta vazio!",
                };
            }
            if (string.IsNullOrEmpty(request.email))
            {
                return new ResponseUnlockAccount
                {
                    codeResult = 2,
                    result = "Campo email esta vazio!",
                };
            }
            try
            {
                retorno = this._integration.UnlockAccount(request.email, request.nomeTabela);
                return new ResponseUnlockAccount
                {
                    codeResult = retorno.Key,
                    result = retorno.Value
                };
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine(ex.Message, TraceLevel.Error);
                return new ResponseUnlockAccount
                {
                    codeResult = 2,
                    result = $"Requisição falhou, erro: {ex.Message}",
                };
            }
        }
        #endregion

        #region ActiveAccount
        /// <summary> 
        /// configuracoes do endpoint de ativar usuario
        /// </summary>
        public ResponseActiveAccount ActiveAccount(RequestActiveAccount request)
        {
            this._setCommons();
            if (string.IsNullOrEmpty(request.nomeTabela))
            {
                return new ResponseActiveAccount
                {
                    codeResult = 2,
                    result = "Campo nomeTabela esta vazio!",
                };
            }
            if (string.IsNullOrEmpty(request.email))
            {
                return new ResponseActiveAccount
                {
                    codeResult = 2,
                    result = "Campo email esta vazio!",
                };
            }
            try
            {
                retorno = this._integration.ActiveAccount(request.email, request.nomeTabela);
                return new ResponseActiveAccount
                {
                    codeResult = retorno.Key,
                    result = retorno.Value
                };
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine(ex.Message, TraceLevel.Error);
                return new ResponseActiveAccount
                {
                    codeResult = 2,
                    result = $"Requisição falhou, erro: {ex.Message}",
                };
            }
        }
        #endregion

        #region RecoverPassword
        /// <summary> 
        /// configuracoes do endpoint de ativar usuario
        /// </summary>
        public ResponseRecoverPassword RecoverPassword(RequestRecoverPassword request)
        {
            this._setCommons();
            if (string.IsNullOrEmpty(request.nomeTabela))
            {
                return new ResponseRecoverPassword
                {
                    codeResult = 2,
                    result = "Campo nomeTabela esta vazio!",
                };
            }
            if (string.IsNullOrEmpty(request.email))
            {
                return new ResponseRecoverPassword
                {
                    codeResult = 2,
                    result = "Campo email esta vazio!",
                };
            }
            try
            {
                retorno = this._integration.RecoverPassword(request.email, request.nomeTabela);
                return new ResponseRecoverPassword
                {
                    codeResult = retorno.Key,
                    result = retorno.Value
                };
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine(ex.Message, TraceLevel.Error);
                return new ResponseRecoverPassword
                {
                    codeResult = 2,
                    result = $"Requisição falhou, erro: {ex.Message}",
                };
            }
        }
        #endregion

        #region ChangePassword
        /// <summary> 
        /// configuracoes do endpoint de recuperar senha do usuario
        /// </summary>

        public ResponseChangePassword ChangePassword(RequestChangePassword request)
        {
            this._setCommons();

            if (string.IsNullOrEmpty(request.email))
            {
                return new ResponseChangePassword
                {
                    codeResult = 2,
                    result = "Campo email esta vazio!",
                };
            }
            if (string.IsNullOrEmpty(request.senha))
            {
                return new ResponseChangePassword
                {
                    codeResult = 2,
                    result = "Campo senha esta vazio!",
                };
            }
            if (string.IsNullOrEmpty(request.senhaNova))
            {
                return new ResponseChangePassword
                {
                    codeResult = 2,
                    result = "Campo senhaNova esta vazio!",
                };
            }
            else
            {
                Regex regex = new Regex(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[$*&@#])[0-9a-zA-Z$*&@#]{8,}$");
                Match match = regex.Match(request.senhaNova);
                if (!match.Success)
                {
                    return new ResponseChangePassword
                    {
                        codeResult = 2,
                        result = "Campo senhaNova não é forte o suficiente, minimo oito caracteres, uma letra maiuscula, um numero, 1 simbulo entre $*&@# e sem caracteres sequenciais como aa, bb, 44, etc!",
                    };
                }
            }

            if (string.IsNullOrEmpty(request.confirmarSenha))
            {
                return new ResponseChangePassword
                {
                    codeResult = 2,
                    result = "Campo confirmar senha esta vazio!",
                };
            }
            else if (request.senhaNova != request.confirmarSenha)
            {
                return new ResponseChangePassword
                {
                    codeResult = 2,
                    result = "Senha não confere!",
                };
            }
            if (string.IsNullOrEmpty(request.nomeTabela))
            {
                return new ResponseChangePassword
                {
                    codeResult = 2,
                    result = "Campo nomeTabela esta vazio!",
                };
            }

            try
            {
                retorno = this._integration.ChangePassword(request.email, request.senha, request.senhaNova, request.nomeTabela);
                return new ResponseChangePassword
                {
                    codeResult = retorno.Key,
                    result = retorno.Value
                };
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine(ex.Message, TraceLevel.Error);
                return new ResponseChangePassword
                {
                    codeResult = 2,
                    result = $"Requisição falhou, erro: {ex.Message}",
                };
            }
        }
        #endregion

        #endregion
    }
}
