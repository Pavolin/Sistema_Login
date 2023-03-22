using API.Arguments.Login;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace API.Routes
{
    [ServiceContract]
    public interface ILogin
    {
        /// <summary> 
        /// Configuracao do roteamento do endpoit, metodo e referencia
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "", Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ResponseLogin Login(RequestLogin request);
    }
}
