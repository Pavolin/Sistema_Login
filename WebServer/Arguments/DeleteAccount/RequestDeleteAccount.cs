using System.Runtime.Serialization;

using API.Interfaces;

namespace API.Arguments.DeleteAccount
{
    [DataContract]
    public class RequestDeleteAccount : IRequest
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
