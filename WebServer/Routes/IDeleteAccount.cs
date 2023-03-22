using System.ServiceModel;
using System.ServiceModel.Web;

using API.Arguments.DeleteAccount;

namespace API.Routes
{
    [ServiceContract]
    public interface IDeleteAccount
    {
        /// <summary> 
        /// Configuracao do roteamento do endpoit, metodo e referencia
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "", Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ResponseDeleteAccount DeleteAccount(RequestDeleteAccount request);
    }
}
