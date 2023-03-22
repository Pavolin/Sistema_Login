
# Sistema Login v1.0

API Rest com endpoints para realizar todos os processos de um sistema de login, cadastrar e logar usuario, ativar, bloquear e desbloquear conta, recuperar dados de acesso e  permissionamento.
Contamos com encripitamento das senhas usadas dentro dessa API tanto no authorization como senhas das contas dos usuarios.




## Documentação

[Documentação API](https://documentacao-api.glitch.me/)

[UML e Fluxo Grama](https://drive.google.com/file/d/1MIPLNcax-1mJ3A1ZQH9xpkoC0IwQ5Ss4/view)

[Coleção chamadas POSTMAN](https://drive.google.com/file/d/1NCn9CtY1LUAXUMfDELFdCuo_WinBM-lI/view?usp=sharing)
## Variáveis de Ambiente

Para rodar esse projeto, você vai precisar adicionar as seguintes variáveis de ambiente no seu .config

LOG

- `tracePath` - variavel onde ira ser gravado os arquivos de LOG

- `enable` - booleano para ativar/desativar LOG

- `traceLevel` - level do LOG (Debug,Critical,Error,Warning,Notice,Info,Full)

- `maxSize` - variavel para definir o tamanho maximo do arquivo de LOG

- `detailedException` - booleano para ativar/desativar os detales dos erros Exception

WebServer

- `baseAddress` - endereço HTTP da API

- `bearerToken` - token de autenticacao da API (colocar token ja criptografado)

DBO

- `serverName` - nome do servidor DBO

- `dataBaseName``- nome do bando de dados

- `userName` - nome de usuario para se autenticar ao DBO

- `password` - senha do usuario para se autenticar ao DBO

- `integratedSecurity` - booleano caso esteja TRUE irá usar a antenticacao do windows, caso FALSE usara o usaurio e senha colocado no .config
## Funcionalidades

- Login
- Cadastro
- Mudar permisao
- Ativar conta
- Bloquear conta
- Desbloquear conta
- Recuperar senha
- Criar tabela
- Alterar senha
- Excluir usuario 
- Gerar token criptografado


## Instalação

Instale o serviço com o CMD, usar o exe da pasta Projeto_SistemaLogin\SistemLogin\bin\Release

```bash
  cd C:\Windows\Microsoft.NET\Framework\v4.0.30319\
  InstallUtil.exe “~\Projeto_SistemaLogin\SistemLogin\bin\Release\SistemaLogin.exe”
```

Abra o serivice.msc e inicie o seriviço SistemaLogin

## Desistalação

Desistale o serviço com o CMD

```bash
  sc delete SistemaLogin
```
    
## Rodando os testes

Para rodar os testes, execute o SistemLogin.sln no visual Studio e executar os testes com as seguintes opções:

![Run testes](https://i.ibb.co/Wc2SZKk/download.png)

Run all tests - ira rodar todos os testes 
Debug all tests - ira rodar todos os testes com o retorno dos testes mais detalhado




## Autores

- GitHub [@Pavolin](https://github.com/Pavolin)
- Linkedin [@Gabriel Pavolin](https://www.linkedin.com/in/gabriel-pavolin-5327a9136)

