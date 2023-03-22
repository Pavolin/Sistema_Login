using System.Collections.Generic;

namespace Integration.Interfaces
{
    public interface IIntegration 
    {
        #region funcoes
        /// <summary> 
        /// camada de integracao das funcoes usadas na API
        /// </summary>
        KeyValuePair<int, string> CreateTable(string nomeTabela);
        KeyValuePair<int, string> RegisterUser(string email, string nome, string senha, string telefone, string nomeTabela);
        KeyValuePair<int, string> Login(string email, string senha, string nomeTabela);
        KeyValuePair<int, string> GeneratorToken(string token);
        KeyValuePair<int, string> ChangePermission(string email, string permissao, string nomeTabela);
        KeyValuePair<int, string> DeleteAccount(string email, string nomeTabela);
        KeyValuePair<int, string> BlockAccount(string email, string nomeTabela);
        KeyValuePair<int, string> UnlockAccount(string email, string nomeTabela);
        KeyValuePair<int, string> ActiveAccount(string email, string nomeTabela);
        KeyValuePair<int, string> RecoverPassword(string email, string nomeTabela);
        KeyValuePair<int, string> ChangePassword(string email, string senha, string senhaNova, string nomeTabela);
        #endregion
    }
}