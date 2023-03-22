using System.ServiceModel;
using System.ServiceModel.Web;

using API.Arguments.ChangePassword;

namespace API.Routes
{
    [ServiceContract]
    public interface IChangePassword
    {
        /// <summary> 
        /// Configuracao do roteamento do endpoit, metodo e referencia
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "", Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ResponseChangePassword ChangePassword(RequestChangePassword request);
    }
}
