using System.ServiceModel;
using System.ServiceModel.Web;

using API.Arguments.RecoverPassword;

namespace API.Routes
{
    [ServiceContract]
    public interface IRecoverPassword
    {
        /// <summary> 
        /// Configuracao do roteamento do endpoit, metodo e referencia
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "", Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ResponseRecoverPassword RecoverPassword(RequestRecoverPassword request);
    }
}
