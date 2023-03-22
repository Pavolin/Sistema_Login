using API.Interfaces;
using System.Runtime.Serialization;

namespace API.Arguments.Login 
{
    [DataContract]
    public class ResponseLogin : IResponse
    {
        /// <summary> 
        /// variaveis de armazenamento das informacoes recebidas para enviar pro response rest
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string result { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int codeResult { get; set; }
    }
}
