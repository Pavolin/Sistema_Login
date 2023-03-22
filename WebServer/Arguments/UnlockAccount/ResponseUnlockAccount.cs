using System.Runtime.Serialization;
using API.Interfaces;

namespace API.Arguments.UnlockAccount
{
    [DataContract]
    public class ResponseUnlockAccount : IResponse
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
