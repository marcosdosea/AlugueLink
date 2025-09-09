# Instru��es de configura��o e execu��o � AlugueLinkWEB

Este documento descreve os passos para configurar e executar este reposit�rio localmente.

Premissas do cen�rio informado:
- Existe um banco `aluguelink` (base principal) � pode ser criado a partir de um arquivo .mwb ou manualmente.
- A configura��o padr�o no projeto usa usu�rio `root` e senha `123456` para MySQL; ajuste se necess�rio.

> Observa��o: as configura��es de conex�o est�o em `AlugueLinkWEB/appsettings.json` (por padr�o apontam para `localhost:3306`, usu�rio `root` e senha `123456`).

---

## 1) Pr�-requisitos (instalar se necess�rio)
- .NET 8 SDK: https://dotnet.microsoft.com/download
- MySQL Server (rodando localmente) e cliente (MySQL Workbench opcional)
- dotnet-ef tool (caso n�o tenha):

  Windows / macOS / Linux:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

  Ou atualizar:
  ```bash
  dotnet tool update --global dotnet-ef
  ```

- (Opcional) Visual Studio 2022 / VS Code com C#

---

## 2) Abrir o projeto
Abra a pasta do reposit�rio no terminal ou na IDE. Exemplo de caminho local:
```bash
cd C:\Projetos\AlugueLink\Codigo
```

---

## 3) Restaurar pacotes
```bash
dotnet restore
```

---

## 4) Criar o banco do Identity (se ainda n�o existir)
O projeto usa um banco separado para Identity chamado `IdentityUsers`.
No MySQL execute:

```sql
CREATE DATABASE IF NOT EXISTS IdentityUsers CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

> Se j� existir, apenas confirme que as credenciais e host em `appsettings.json` batem com a instala��o do MySQL.

---

## 5) Verificar / ajustar ConnectionStrings (opcional)
Abra `AlugueLinkWEB/appsettings.json` e confirme as connection strings:
```json
"ConnectionStrings": {
  "AluguelinkDatabase": "server=localhost;port=3306;user=root;password=123456;database=aluguelink",
  "IdentityDatabase": "server=localhost;port=3306;user=root;password=123456;database=IdentityUsers"
}
```
Substitua `123456` se a senha do MySQL for diferente.

---

## 6) Aplicar migra��es / criar esquema do Identity
Navegue para o projeto web e execute as migra��es:

```bash
cd AlugueLinkWEB

# Aplicar migra��es existentes para o contexto Identity
dotnet ef database update --context IdentityContext
```

- Se o reposit�rio j� cont�m migra��es, esse comando criar� as tabelas do Identity (`AspNetUsers`, `AspNetRoles`, etc.).
- Se n�o houver migra��es no reposit�rio, crie uma migra��o e aplique (apenas se necess�rio):

```bash
# criar migra��o (gera arquivos em Migrations/ para IdentityContext)
dotnet ef migrations add InitialIdentityCreate --context IdentityContext
# aplicar
dotnet ef database update --context IdentityContext
```

---

## 7) Aplicar migra��es / criar esquema do banco principal (Aluguelink)
Se o banco `aluguelink` j� existe e cont�m esquema (por exemplo vindo de um .mwb), talvez n�o seja necess�rio aplicar migra��es; caso queira garantir que as tabelas usadas pelo projeto existam, execute:

```bash
# aplicar migra��es para o contexto principal
dotnet ef database update --context AluguelinkContext
```

Se n�o houver migra��es no reposit�rio, pode ser preciso gerar uma migra��o `InitialAluguelinkCreate` e rod�-la (opcional).

---

## 8) Build e execu��o
```bash
# a partir da pasta AlugueLinkWEB
dotnet build
dotnet run
```
A aplica��o abrir� em `https://localhost:5001` (ou `http://localhost:5000`).

---

## 9) Testes r�pidos
- Abra o browser em `https://localhost:5001`.
- Na landing page: clique em **Criar Conta Gr�tis** e registre um usu�rio.
- Fa�a login com o usu�rio criado.
- Verifique que a sidebar aparece e o dashboard funciona.

---

## 10) Observa��es sobre SMTP / envio de email
- O arquivo `appsettings.json` inclui se��o `Smtp` com credenciais para envio de email. No reposit�rio essas credenciais s�o placeholders (`no-reply@aluguelink.com`, `change_me`).
- Para desenvolvimento, n�o � obrigat�rio configurar SMTP � o sistema aceita registros sem confirma��o se a aplica��o estiver configurada assim (ver `Program.cs`).
- Se quiser ativar envio real, use credenciais reais (ou vari�veis de ambiente / user-secrets) e atualize `appsettings.json`.

---

## 11) Solu��o de problemas comuns
- Erro de conex�o MySQL: verifique se o servi�o MySQL est� rodando e credenciais em `appsettings.json`.
- Comando `dotnet ef` n�o encontrado: instale `dotnet-ef` globalmente (veja se��o 1).
- Migration exception: confira permiss�es do usu�rio MySQL (GRANT) e charset/collation do banco.

---

## 12) Resumo dos comandos (r�pido)
```bash
# 1. Restore
dotnet restore

# 2. Criar banco Identity (se necess�rio) - no MySQL
# CREATE DATABASE IdentityUsers ...

# 3. Aplicar migra��es Identity
cd AlugueLinkWEB
dotnet ef database update --context IdentityContext

# 4. Aplicar migra��es do banco principal (opcional)
dotnet ef database update --context AluguelinkContext

# 5. Build e run
dotnet build
dotnet run
```

---


