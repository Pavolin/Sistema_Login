using System.ServiceModel;
using System.ServiceModel.Web;

using API.Arguments.CreateTable;

namespace API.Routes
{
    [ServiceContract]
    public interface ICreateTable
    {
        /// <summary> 
        /// Configuracao do roteamento do endpoit, metodo e referencia
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "", Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ResponseCreateTable CreateTable(RequestCreateTable request);
    }
}
