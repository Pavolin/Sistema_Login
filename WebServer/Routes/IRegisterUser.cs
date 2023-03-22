using System.ServiceModel;
using System.ServiceModel.Web;

using API.Arguments.RegisterUser;

namespace API.Routes
{
    [ServiceContract]
    public interface IRegisterUser
    {
        /// <summary> 
        /// Configuracao do roteamento do endpoit, metodo e referencia
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "", Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ResponseRegisterUser RegisterUser(RequestRegisterUser request);
    }
}
