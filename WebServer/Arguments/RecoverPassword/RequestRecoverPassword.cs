using System.Runtime.Serialization;

using API.Interfaces;

namespace API.Arguments.RecoverPassword
{
    [DataContract]
    public class RequestRecoverPassword : IRequest
    {
        /// <summary> 
        /// variaveis de armazenamento das informacoes recebidas da requisicao rest
        /// </summary>
        [DataMember]
        public string nomeTabela { get; set; }
        [DataMember]
        public string email { get; set; }
    }
}
