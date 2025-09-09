# Instru��es de configura��o e execu��o � AlugueLinkWEB

Este documento descreve os passos m�nimos para configurar e executar este reposit�rio localmente. O objetivo � deixar o projeto pronto para um novo usu�rio com o menor n�mero de passos poss�veis.

Premissas e conven��es usadas neste reposit�rio:
- O script SQL completo de cria��o do banco est� em: sql/scriptDatabase_Aluguelink.sql � ele cria o schema `aluguelink` e tamb�m o schema `IdentityUsers` com as tabelas do ASP.NET Identity prontas para uso.
- A configura��o padr�o usa MySQL em `localhost:3306` com usu�rio `root` e senha `123456`. Se o seu ambiente for diferente, ajuste `AlugueLinkWEB/appsettings.json` antes de executar.
- **Este projeto N�O usa migrations** - apenas o script SQL como fonte �nica da verdade do banco.

---

## 1) Pr�-requisitos
- .NET 8 SDK: https://dotnet.microsoft.com/download
- MySQL Server (rodando localmente) e cliente (MySQL Workbench recomendado)

---

## 2) Preparar o banco (passo �nico obrigat�rio)
1. Abra o MySQL Workbench (ou outro cliente) conectado ao seu servidor MySQL (padr�o: localhost:3306, usu�rio: root, senha: 123456).
2. Abra o arquivo `sql/scriptDatabase_Aluguelink.sql` e execute-o. Este script ir�:
   - Criar o schema `aluguelink` e todas as tabelas necess�rias pela aplica��o (locador, imovel, locatario, aluguel, manutencao, pagamento, etc.).
   - Criar o schema `IdentityUsers` com as tabelas do ASP.NET Identity completas.
   - Inserir dados iniciais necess�rios e deixar o banco pronto para uso.

**?? IMPORTANTE: Este � o �nico m�todo suportado para cria��o do banco. Migrations foram removidas do projeto.**

---

## 3) Ajustar connection strings (se necess�rio)
Abra `AlugueLinkWEB/appsettings.json` e verifique as connection strings:

```json
"ConnectionStrings": {
  "AluguelinkDatabase": "server=localhost;port=3306;user=root;password=123456;database=aluguelink",
  "IdentityDatabase": "server=localhost;port=3306;user=root;password=123456;database=IdentityUsers"
}
```

Altere `user`/`password`/`server` se o seu MySQL n�o usar as credenciais padr�o.

---

## 4) Build e execu��o
No terminal, a partir da pasta raiz do reposit�rio (`Codigo`):

```bash
cd AlugueLinkWEB
dotnet restore
dotnet build
dotnet run
```

A aplica��o estar� dispon�vel em `https://localhost:5001` (ou `http://localhost:5000`).

---

## 5) Acesso inicial
- Um usu�rio administrador � criado automaticamente durante a inicializa��o da aplica��o (se n�o existir).
- Credenciais padr�o configuradas em `appsettings.json` para facilitar testes:
  - Email: `admin@aluguelink.com`
  - Senha: `Admin1234!`

Se desejar alterar essas credenciais, edite a se��o `AdminUser` em `AlugueLinkWEB/appsettings.json` antes de rodar a aplica��o.

---

## Solu��o de problemas r�pidos
- Erro de conex�o MySQL: verifique se o servi�o MySQL est� rodando e as credenciais em `appsettings.json`.
- Script SQL falha com erros de permiss�o: verifique se o usu�rio MySQL tem permiss�es para criar bases e tabelas (GRANT).
- Usu�rio admin n�o aparece: confira os logs da aplica��o; o seeding de dados tenta criar a role `Administrator` e o usu�rio admin se necess�rio.

---

## Resumo r�pido (fluxo obrigat�rio)
1. Executar `sql/scriptDatabase_Aluguelink.sql` no MySQL Workbench (usu�rio root / senha 123456)
2. cd AlugueLinkWEB
3. dotnet restore
4. dotnet build
5. dotnet run

**Nota:** Migrations foram removidas - use apenas o script SQL.




