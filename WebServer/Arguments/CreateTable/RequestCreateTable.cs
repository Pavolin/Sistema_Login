using System.Runtime.Serialization;

using API.Interfaces;

namespace API.Arguments.CreateTable
{
    [DataContract]
    public class RequestCreateTable : IRequest
    {
        /// <summary> 
        /// variaveis de armazenamento das informacoes recebidas da requisicao rest
        /// </summary>
        [DataMember]
        public string nomeTabela { get; set; }
    }
}
