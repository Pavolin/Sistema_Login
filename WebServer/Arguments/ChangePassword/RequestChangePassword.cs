using System.Runtime.Serialization;

using API.Interfaces;

namespace API.Arguments.ChangePassword
{
    [DataContract]
    public class RequestChangePassword : IRequest
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
        [DataMember]
        public string senhaNova { get; set; }
        [DataMember]
        public string confirmarSenha { get; set; }
    }
}
