using System.Runtime.Serialization;


namespace API.Arguments.RegisterUser
{
    [DataContract]
    public class RequestRegisterUser
    {
        /// <summary> 
        /// variaveis de armazenamento das informacoes recebidas da requisicao rest
        /// </summary>
        [DataMember]
        public string email { get; set; }
        [DataMember]
        public string senha { get; set; }
        [DataMember]
        public string confirmarSenha { get; set; }
        [DataMember]
        public string nome { get; set; }
        [DataMember]
        public string telefone { get; set; }
        [DataMember]
        public string nomeTabela { get; set; }
    }
}
