using Integration.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Utils.Enums;
using Utils.Interfaces;

namespace Integration.Services
{
    public class Funcoes : IIntegration
    {
        #region Atributos
        /// <summary> 
        /// area das referencias ao atributos usados nessa classe
        /// </summary>
        private readonly ITracer _tracer;
        private string serverName;
        private string dataBaseName;
        private string userName;
        private string password;
        private bool integratedSecurity;
        private SqlConnection con = new SqlConnection();
        private readonly IAesEncryptor encripitador;
        #endregion

        #region Constructor Methods
        /// <summary> 
        /// area das configuracao dos metodos referenciado na classe SLinit
        /// </summary>
        public Funcoes(ITracer tracer, string serverName, string dataBaseName, string userName, string password, bool integratedSecurity, IAesEncryptor encripitador)
        {
            this._tracer = tracer;
            this.serverName = serverName;
            this.dataBaseName = dataBaseName;
            this.userName = userName;
            this.password = password;
            this.integratedSecurity = integratedSecurity;
            this.encripitador = encripitador;
        }
        #endregion

        #region metodos
        /// <summary> 
        /// area das funcoes usadas na API
        /// </summary>
        private string _conectarBanco()
        {
            try
            {
                if (this.integratedSecurity)
                {
                    con.ConnectionString =
                        $"Data Source={serverName};" +
                        $"Initial Catalog={dataBaseName};" +
                        $"Integrated Security = {integratedSecurity};";
                    con.Open();
                }
                else 
                {
                    con.ConnectionString =
                        $"Data Source={serverName};" +
                        $"Initial Catalog={dataBaseName};" +
                        $"User id={userName};" +
                        $"Password={password};";
                    con.Open();
                }
                
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine($"{ex.Message}", TraceLevel.Error);
                return ex.Message;
            }

            return "banco conectato com sucesso";
        }

        private string _desconectarBanco()
        {
            try
            {
                SqlConnection con = new SqlConnection();
                con.Close();
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine($"{ex.Message}", TraceLevel.Error);
                return ex.Message;
            }

            return "banco desconectado com sucesso";
        }

        public KeyValuePair<int, string> CreateTable(string nomeTabela)
        {
            this._tracer.WriteLine($"CreateTable()", TraceLevel.Debug);

            try
            {
                string retornoConexao = this._conectarBanco();

                this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
            
                this._tracer.WriteLine($"Criado tabela...", TraceLevel.Info);
                SqlCommand create = new SqlCommand($"CREATE TABLE {nomeTabela}_usuario(email VARCHAR(50) NOT NULL, senha VARCHAR(50) NOT NULL, nome VARCHAR(50) NOT NULL, primeiroNome VARCHAR(25), ultimoNome Varchar(25), permissao VARCHAR(25) NOT NULL, telefone VARCHAR(50) NOT NULL, ativado BIT NOT NULL);", con);
                create.ExecuteNonQuery();
                this._tracer.WriteLine($"Tabela criada com sucesso!", TraceLevel.Info);

                retornoConexao = this._desconectarBanco();
                this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine($"{ex.Message}", TraceLevel.Error);
                string retornoConexao = this._desconectarBanco();
                this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                return new KeyValuePair<int, string>(2, ex.Message);
            }

            return new KeyValuePair<int, string>(1, $"Tabela {nomeTabela}_usuario criada com sucesso!");
        }

        public KeyValuePair<int, string> RegisterUser(string email, string nome, string senha, string telefone, string nomeTabela)
        {
            this._tracer.WriteLine($"RegisterUser()", TraceLevel.Debug);
            string permissao = "usuario";
            string[] nomeFormatado = nome.Split('.');
            string retornoConexao = "";

            try
            {
                retornoConexao = this._conectarBanco();

                this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);

                this._tracer.WriteLine($"Registrando usuario...", TraceLevel.Info);
                SqlCommand create = new SqlCommand($"SELECT COUNT(*) FROM {nomeTabela}_usuario;", con);
                int resultado = Convert.ToInt32(create.ExecuteScalar());

                if(resultado == 0)
                {
                    permissao = "administrador";
                }
                else
                {
                    create = new SqlCommand($"SELECT COUNT(*) FROM {nomeTabela}_usuario WHERE email = '{email}'", con);
                    resultado = Convert.ToInt32(create.ExecuteScalar());

                    if(resultado > 0)
                    {
                        this._tracer.WriteLine($"Usuario registrado anteriormente!", TraceLevel.Error);
                        retornoConexao = this._desconectarBanco();
                        this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                        return new KeyValuePair<int, string>(2, $"Usuario {email} ja cadastrado anteriormente!");
                    }
                }

                string senhaEncripitada = encripitador.Encrypt(senha);

                create = new SqlCommand($"Insert into {nomeTabela}_usuario (email, senha, nome, primeiroNome, ultimoNome, permissao, telefone, ativado) VALUES ('{email}', '{senhaEncripitada}', '{nome}', '{nomeFormatado[0]}', '{nomeFormatado[1]}', '{permissao}', '{telefone}', 0);", con);
                create.ExecuteNonQuery();

                this._tracer.WriteLine($"Usuario registrado com sucesso!", TraceLevel.Info);

                retornoConexao = this._desconectarBanco();
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine($"{ex.Message}", TraceLevel.Error);
                retornoConexao = this._desconectarBanco();
                this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                return new KeyValuePair<int, string>(2, ex.Message);
            }

            this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);

            return new KeyValuePair<int, string>(1, $"Usuario {email} registrado com sucesso!");
        }

        public KeyValuePair<int, string> Login(string email, string senha, string nomeTabela)
        {
            this._tracer.WriteLine($"Login()", TraceLevel.Debug);
            string retornoConexao = "";
            string[] retornoSelect = new string[3];
            bool ativado = false;

            try
            {
                retornoConexao = this._conectarBanco();

                this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);

                this._tracer.WriteLine($"Realizando login...", TraceLevel.Info);
                SqlCommand create = new SqlCommand($"SELECT COUNT(*) FROM {nomeTabela}_usuario WHERE email = '{email}';", con);
                int resultado = Convert.ToInt32(create.ExecuteScalar());

                if (resultado == 0)
                {
                    this._tracer.WriteLine($"Usuario nao encontrado", TraceLevel.Info);
                    retornoConexao = this._desconectarBanco();
                    this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                    return new KeyValuePair<int, string>(2, $"Usuario {email} não encontrado, verifique o email preenchido!");
                }
                else
                {
                    create = new SqlCommand($"SELECT email FROM {nomeTabela}_usuario WHERE email = '{email}'", con);
                    retornoSelect[0] = Convert.ToString(create.ExecuteScalar());
                    create = new SqlCommand($"SELECT senha FROM {nomeTabela}_usuario WHERE email = '{email}'", con);
                    retornoSelect[1] = Convert.ToString(create.ExecuteScalar());
                    create = new SqlCommand($"SELECT ativado FROM {nomeTabela}_usuario WHERE email = '{email}'", con);
                    ativado = Convert.ToBoolean(create.ExecuteScalar());
                    create = new SqlCommand($"SELECT permissao FROM {nomeTabela}_usuario WHERE email = '{email}'", con);
                    retornoSelect[2] = Convert.ToString(create.ExecuteScalar());

                    string senhaDescripitada = encripitador.Decrypt(retornoSelect[1]);

                    if (senha == senhaDescripitada)
                    {
                        if(!ativado)
                        {
                            this._tracer.WriteLine($"usuario inativo!", TraceLevel.Info);
                            retornoConexao = this._desconectarBanco();
                            this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                            return new KeyValuePair<int, string>(2, "Usuario inativo por favor ativar conta!");
                        }
                        else if(retornoSelect[2] == "")
                        {
                            this._tracer.WriteLine($"usuario bloqueado!", TraceLevel.Info);
                            retornoConexao = this._desconectarBanco();
                            this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                            return new KeyValuePair<int, string>(2, "Usuario bloqueado por favor entrar em contato com o supervisor ou suporte!");
                        }

                        this._tracer.WriteLine($"Usuario logado com sucesso!", TraceLevel.Info);
                    }
                    else 
                    {
                        this._tracer.WriteLine($"Senha invalida!", TraceLevel.Info);
                        retornoConexao = this._desconectarBanco();
                        this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                        return new KeyValuePair<int, string>(2, "Senha invalida!");
                    }

                }

                retornoConexao = this._desconectarBanco();
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine($"{ex.Message}", TraceLevel.Error);
                retornoConexao = this._desconectarBanco();
                this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                return new KeyValuePair<int, string>(2, ex.Message);
            }

            this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);

            return new KeyValuePair<int, string>(1, $"Usuario {email} logado com sucesso!");
        }

        public KeyValuePair<int, string> GeneratorToken(string token)
        {
            this._tracer.WriteLine($"GeneratorToken()", TraceLevel.Debug);
            string tokenEncripitado = "";

            try
            {
                tokenEncripitado = encripitador.Encrypt(token);
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine($"{ex.Message}", TraceLevel.Error);
                return new KeyValuePair<int, string>(2, ex.Message);
            }

            return new KeyValuePair<int, string>(1, $"Token {token} encrepitado com sucesso: {tokenEncripitado}!");
        }

        public KeyValuePair<int, string> ChangePermission(string email, string permissao, string nomeTabela)
        {
            this._tracer.WriteLine($"ChangePermission()", TraceLevel.Debug);
            string retornoConexao = "";

            if(permissao != "administrador" && permissao != "supervisor" && permissao != "usuario")
            {
                this._tracer.WriteLine("permissao invalida, permissoes aceitas: administrador, supervisor ou usuario", TraceLevel.Info);
                return new KeyValuePair<int, string>(2, "permissao invalida, permissoes aceitas: administrador, supervisor ou usuario");
            }

            try
            {
                retornoConexao = this._conectarBanco();

                this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);

                this._tracer.WriteLine($"Procurando usuario...", TraceLevel.Info);
                SqlCommand create = new SqlCommand($"SELECT COUNT(*) FROM {nomeTabela}_usuario WHERE email = '{email}';", con);
                int resultado = Convert.ToInt32(create.ExecuteScalar());

                if (resultado == 0)
                {
                    this._tracer.WriteLine($"Usuario nao encontrado", TraceLevel.Info);
                    retornoConexao = this._desconectarBanco();
                    this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                    return new KeyValuePair<int, string>(2, $"Usuario {email} não encontrado, verifique o email preenchido!");
                }

                this._tracer.WriteLine($"Alterando permissao...", TraceLevel.Info);
                create = new SqlCommand($"UPDATE {nomeTabela}_usuario SET permissao = '{permissao}' WHERE email = '{email}';", con);
                create.ExecuteNonQuery();

                retornoConexao = this._desconectarBanco();
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine($"{ex.Message}", TraceLevel.Error);
                retornoConexao = this._desconectarBanco();
                this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                return new KeyValuePair<int, string>(2, ex.Message);
            }

            this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);

            return new KeyValuePair<int, string>(1, $"Permissao do usuario {email} alterado com sucesso!");
        }

        public KeyValuePair<int, string> DeleteAccount(string email, string nomeTabela)
        {
            this._tracer.WriteLine($"DeleteAccount()", TraceLevel.Debug);
            string retornoConexao = "";

            try
            {
                retornoConexao = this._conectarBanco();

                this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);

                this._tracer.WriteLine($"Procurando usuario...", TraceLevel.Info);
                SqlCommand create = new SqlCommand($"SELECT COUNT(*) FROM {nomeTabela}_usuario WHERE email = '{email}';", con);
                int resultado = Convert.ToInt32(create.ExecuteScalar());

                if (resultado == 0)
                {
                    this._tracer.WriteLine($"Usuario nao encontrado", TraceLevel.Info);
                    retornoConexao = this._desconectarBanco();
                    this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                    return new KeyValuePair<int, string>(2, $"Usuario {email} não encontrado, verifique o email preenchido!");
                }

                this._tracer.WriteLine($"Excluindo Usuario...", TraceLevel.Info);
                create = new SqlCommand($"DELETE FROM {nomeTabela}_usuario WHERE email = '{email}';", con);
                create.ExecuteNonQuery();

                retornoConexao = this._desconectarBanco();
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine($"{ex.Message}", TraceLevel.Error);
                retornoConexao = this._desconectarBanco();
                this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                return new KeyValuePair<int, string>(2, ex.Message);
            }

            this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);

            return new KeyValuePair<int, string>(1, $"Usuario {email} excluido com sucesso!");
        }

        public KeyValuePair<int, string> BlockAccount(string email, string nomeTabela)
        {
            this._tracer.WriteLine($"BlockAccount()", TraceLevel.Debug);
            string retornoConexao = "";

            try
            {
                retornoConexao = this._conectarBanco();

                this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);

                this._tracer.WriteLine($"Procurando usuario...", TraceLevel.Info);
                SqlCommand create = new SqlCommand($"SELECT COUNT(*) FROM {nomeTabela}_usuario WHERE email = '{email}';", con);
                int resultado = Convert.ToInt32(create.ExecuteScalar());

                if (resultado == 0)
                {
                    this._tracer.WriteLine($"Usuario nao encontrado", TraceLevel.Info);
                    retornoConexao = this._desconectarBanco();
                    this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                    return new KeyValuePair<int, string>(2, $"Usuario {email} não encontrado, verifique o email preenchido!");
                }

                this._tracer.WriteLine($"Bloqueando Usuario...", TraceLevel.Info);
                create = new SqlCommand($"UPDATE {nomeTabela}_usuario SET permissao = '' WHERE email = '{email}';", con);
                create.ExecuteNonQuery();

                retornoConexao = this._desconectarBanco();
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine($"{ex.Message}", TraceLevel.Error);
                retornoConexao = this._desconectarBanco();
                this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                return new KeyValuePair<int, string>(2, ex.Message);
            }

            this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);

            return new KeyValuePair<int, string>(1, $"Usuario {email} bloquado com sucesso!");
        }

        public KeyValuePair<int, string> UnlockAccount(string email, string nomeTabela)
        {
            this._tracer.WriteLine($"UnlockAccount()", TraceLevel.Debug);
            string retornoConexao = "";

            try
            {
                retornoConexao = this._conectarBanco();

                this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);

                this._tracer.WriteLine($"Procurando usuario...", TraceLevel.Info);
                SqlCommand create = new SqlCommand($"SELECT COUNT(*) FROM {nomeTabela}_usuario WHERE email = '{email}';", con);
                int resultado = Convert.ToInt32(create.ExecuteScalar());

                if (resultado == 0)
                {
                    this._tracer.WriteLine($"Usuario nao encontrado", TraceLevel.Info);
                    retornoConexao = this._desconectarBanco();
                    this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                    return new KeyValuePair<int, string>(2, $"Usuario {email} não encontrado, verifique o email preenchido!");
                }

                this._tracer.WriteLine($"Desploqueando Usuario...", TraceLevel.Info);
                create = new SqlCommand($"UPDATE {nomeTabela}_usuario SET permissao = 'usuario' WHERE email = '{email}';", con);
                create.ExecuteNonQuery();

                retornoConexao = this._desconectarBanco();
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine($"{ex.Message}", TraceLevel.Error);
                retornoConexao = this._desconectarBanco();
                this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                return new KeyValuePair<int, string>(2, ex.Message);
            }

            this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);

            return new KeyValuePair<int, string>(1, $"Usuario {email} desbloqueado com sucesso!");
        }

        public KeyValuePair<int, string> ActiveAccount(string email, string nomeTabela)
        {
            this._tracer.WriteLine($"ActiveAccount()", TraceLevel.Debug);
            string retornoConexao = "";

            try
            {
                retornoConexao = this._conectarBanco();

                this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);

                this._tracer.WriteLine($"Procurando usuario...", TraceLevel.Info);
                SqlCommand create = new SqlCommand($"SELECT COUNT(*) FROM {nomeTabela}_usuario WHERE email = '{email}';", con);
                int resultado = Convert.ToInt32(create.ExecuteScalar());

                if (resultado == 0)
                {
                    this._tracer.WriteLine($"Usuario nao encontrado", TraceLevel.Info);
                    retornoConexao = this._desconectarBanco();
                    this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                    return new KeyValuePair<int, string>(2, $"Usuario {email} não encontrado, verifique o email preenchido!");
                }

                this._tracer.WriteLine($"Ativando conta...", TraceLevel.Info);

                create = new SqlCommand($"SELECT ativado FROM {nomeTabela}_usuario WHERE email = '{email}';", con);
                bool resultadoSelect = Convert.ToBoolean(create.ExecuteScalar());

                if (resultadoSelect)
                {
                    this._tracer.WriteLine($"Usuario ja esta ativado", TraceLevel.Info);
                    retornoConexao = this._desconectarBanco();
                    this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                    return new KeyValuePair<int, string>(2, $"Usuario {email} ja esta ativado!");
                }


                create = new SqlCommand($"UPDATE {nomeTabela}_usuario SET ativado = 1 WHERE email = '{email}';", con);
                create.ExecuteNonQuery();

                retornoConexao = this._desconectarBanco();
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine($"{ex.Message}", TraceLevel.Error);
                retornoConexao = this._desconectarBanco();
                this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                return new KeyValuePair<int, string>(2, ex.Message);
            }

            this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);

            return new KeyValuePair<int, string>(1, $"Usuario {email} ativado com sucesso!");
        }

        public KeyValuePair<int, string> RecoverPassword(string email, string nomeTabela)
        {
            this._tracer.WriteLine($"RecoverPassword()", TraceLevel.Debug);
            string retornoConexao = "";
            string senhaDesencripitada = "";

            try
            {
                retornoConexao = this._conectarBanco();

                this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);

                this._tracer.WriteLine($"Procurando usuario...", TraceLevel.Info);
                SqlCommand create = new SqlCommand($"SELECT COUNT(*) FROM {nomeTabela}_usuario WHERE email = '{email}';", con);
                int resultado = Convert.ToInt32(create.ExecuteScalar());

                if (resultado == 0)
                {
                    this._tracer.WriteLine($"Usuario nao encontrado", TraceLevel.Info);
                    retornoConexao = this._desconectarBanco();
                    this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                    return new KeyValuePair<int, string>(2, $"Usuario {email} não encontrado, verifique o email preenchido!");
                }

                this._tracer.WriteLine($"Recuperando senha...", TraceLevel.Info);

                create = new SqlCommand($"SELECT ativado FROM {nomeTabela}_usuario WHERE email = '{email}';", con);
                bool resultadoSelect = Convert.ToBoolean(create.ExecuteScalar());

                if (!resultadoSelect)
                {
                    this._tracer.WriteLine($"Usuario esta inativo, por favor ativar usuario para poder recuperar a senha!", TraceLevel.Info);
                    retornoConexao = this._desconectarBanco();
                    this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                    return new KeyValuePair<int, string>(2, $"Usuario {email} esta inativo, por favor ativar usuario para poder recuperar a senha!");
                }

                create = new SqlCommand($"SELECT senha FROM {nomeTabela}_usuario WHERE email = '{email}';", con);
                string senhaRecuperada = Convert.ToString(create.ExecuteScalar());

                senhaDesencripitada = encripitador.Decrypt(senhaRecuperada);

                retornoConexao = this._desconectarBanco();
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine($"{ex.Message}", TraceLevel.Error);
                retornoConexao = this._desconectarBanco();
                this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                return new KeyValuePair<int, string>(2, ex.Message);
            }

            this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);

            return new KeyValuePair<int, string>(1, $"Senha recuperada com sucesso: {senhaDesencripitada}");
        }

        public KeyValuePair<int, string> ChangePassword(string email, string senha, string senhaNova, string nomeTabela)
        {
            this._tracer.WriteLine($"ChangePassword()", TraceLevel.Debug);
            string retornoConexao = "";
            string senhaDesencripitada = "";

            try
            {
                retornoConexao = this._conectarBanco();

                this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);

                this._tracer.WriteLine($"Procurando usuario...", TraceLevel.Info);
                SqlCommand create = new SqlCommand($"SELECT COUNT(*) FROM {nomeTabela}_usuario WHERE email = '{email}';", con);
                int resultado = Convert.ToInt32(create.ExecuteScalar());

                if (resultado == 0)
                {
                    this._tracer.WriteLine($"Usuario nao encontrado", TraceLevel.Info);
                    retornoConexao = this._desconectarBanco();
                    this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                    return new KeyValuePair<int, string>(2, $"Usuario {email} não encontrado, verifique o email preenchido!");
                }

                this._tracer.WriteLine($"Redefinindo senha...", TraceLevel.Info);

                create = new SqlCommand($"SELECT ativado FROM {nomeTabela}_usuario WHERE email = '{email}';", con);
                bool resultadoSelect = Convert.ToBoolean(create.ExecuteScalar());

                if (!resultadoSelect)
                {
                    this._tracer.WriteLine($"Usuario esta inativo, por favor ativar usuario para poder redefinir a senha!", TraceLevel.Info);
                    retornoConexao = this._desconectarBanco();
                    this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                    return new KeyValuePair<int, string>(2, $"Usuario {email} esta inativo, por favor ativar usuario para poder redefinir a senha!");
                }

                create = new SqlCommand($"SELECT senha FROM {nomeTabela}_usuario WHERE email = '{email}';", con);
                string senhaRecuperada = Convert.ToString(create.ExecuteScalar());

                senhaDesencripitada = encripitador.Decrypt(senhaRecuperada);

                if(senhaDesencripitada != senha)
                {
                    this._tracer.WriteLine($"Senha não confere com a senha atual!", TraceLevel.Info);
                    retornoConexao = this._desconectarBanco();
                    this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                    return new KeyValuePair<int, string>(2, $"Senha {senha} esta incorreta, nao foi possivel alterar a senha!");
                }

                string senhaEncripitada = encripitador.Encrypt(senhaNova);

                create = new SqlCommand($"UPDATE {nomeTabela}_usuario SET senha = '{senhaEncripitada}' WHERE email = '{email}';", con);
                create.ExecuteNonQuery();

                retornoConexao = this._desconectarBanco();
            }
            catch (Exception ex)
            {
                this._tracer.WriteLine($"{ex.Message}", TraceLevel.Error);
                retornoConexao = this._desconectarBanco();
                this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);
                return new KeyValuePair<int, string>(2, ex.Message);
            }

            this._tracer.WriteLine($"{retornoConexao}", TraceLevel.Info);

            return new KeyValuePair<int, string>(1, $"Senha do usuario {email} alterado com sucesso!");
        }

        #endregion


    }
}
