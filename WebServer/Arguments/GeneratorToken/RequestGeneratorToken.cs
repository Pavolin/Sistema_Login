using API.Interfaces;
using System.Runtime.Serialization;


namespace API.Arguments.GeneratorToken
{
    [DataContract]
    public class RequestGeneratorToken : IRequest
    {
        /// <summary> 
        /// variaveis de armazenamento das informacoes recebidas da requisicao rest
        /// </summary>
        [DataMember]
        public string token { get; set; }
    }
}
