using API.Interfaces;
using System.Runtime.Serialization;

namespace API.Arguments.Login
{
    [DataContract]
    public class RequestLogin : IRequest
    {
        /// <summary> 
        /// variaveis de armazenamento das informacoes recebidas da requisicao rest
        /// </summary>
        [DataMember]
        public string nomeTabela { get; set; }
        [DataMember]
        public string email { get; set; }
        [DataMember]
        public string senha { get; set; }
    }
}
