using System.ServiceModel;
using System.ServiceModel.Web;

using API.Arguments.ChangePermission;

namespace API.Routes
{
    [ServiceContract]
    public interface IChangePermission
    {
        /// <summary> 
        /// Configuracao do roteamento do endpoit, metodo e referencia
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "", Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ResponseChangePermission ChangePermission(RequestChangePermission request);
    }
}
