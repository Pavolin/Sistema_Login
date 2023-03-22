using System.ServiceModel;
using System.ServiceModel.Web;

using API.Arguments.ActiveAccount;

namespace API.Routes
{
    [ServiceContract]
    public interface IActiveAccount
    {
        /// <summary> 
        /// Configuracao do roteamento do endpoit, metodo e referencia
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "", Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ResponseActiveAccount ActiveAccount(RequestActiveAccount request);
    }
}
