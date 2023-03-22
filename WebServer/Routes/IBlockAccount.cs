using System.ServiceModel;
using System.ServiceModel.Web;

using API.Arguments.BlockAccount;

namespace API.Routes
{
    [ServiceContract]
    public interface IBlockAccount
    {
        /// <summary> 
        /// Configuracao do roteamento do endpoit, metodo e referencia
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "", Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ResponseBlockAccount BlockAccount(RequestBlockAccount request);
    }
}
