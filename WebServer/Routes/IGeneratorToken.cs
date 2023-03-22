using API.Arguments.GeneratorToken;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace API.Routes
{
    [ServiceContract]
    public interface IGeneratorToken
    {
        /// <summary> 
        /// Configuracao do roteamento do endpoit, metodo e referencia
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "", Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ResponseGeneratorToken GeneratorToken(RequestGeneratorToken request);
    }
}
