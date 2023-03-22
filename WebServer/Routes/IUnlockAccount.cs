using System.ServiceModel;
using System.ServiceModel.Web;

using API.Arguments.UnlockAccount;

namespace API.Routes
{
    [ServiceContract]
    public interface IUnlockAccount
    {
        /// <summary> 
        /// Configuracao do roteamento do endpoit, metodo e referencia
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "", Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ResponseUnlockAccount UnlockAccount(RequestUnlockAccount request);
    }
}
