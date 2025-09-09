# Instruções de configuração e execução — AlugueLinkWEB

Este documento descreve os passos para configurar e executar este repositório localmente.

Premissas do cenário informado:
- Existe um banco `aluguelink` (base principal) — pode ser criado a partir de um arquivo .mwb ou manualmente.
- A configuração padrão no projeto usa usuário `root` e senha `123456` para MySQL; ajuste se necessário.

> Observação: as configurações de conexão estão em `AlugueLinkWEB/appsettings.json` (por padrão apontam para `localhost:3306`, usuário `root` e senha `123456`).

---

## 1) Pré-requisitos (instalar se necessário)
- .NET 8 SDK: https://dotnet.microsoft.com/download
- MySQL Server (rodando localmente) e cliente (MySQL Workbench opcional)
- dotnet-ef tool (caso não tenha):

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
Abra a pasta do repositório no terminal ou na IDE. Exemplo de caminho local:
```bash
cd C:\Projetos\AlugueLink\Codigo
```

---

## 3) Restaurar pacotes
```bash
dotnet restore
```

---

## 4) Criar o banco do Identity (se ainda não existir)
O projeto usa um banco separado para Identity chamado `IdentityUsers`.
No MySQL execute:

```sql
CREATE DATABASE IF NOT EXISTS IdentityUsers CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

> Se já existir, apenas confirme que as credenciais e host em `appsettings.json` batem com a instalação do MySQL.

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

## 6) Aplicar migrações / criar esquema do Identity
Navegue para o projeto web e execute as migrações:

```bash
cd AlugueLinkWEB

# Aplicar migrações existentes para o contexto Identity
dotnet ef database update --context IdentityContext
```

- Se o repositório já contém migrações, esse comando criará as tabelas do Identity (`AspNetUsers`, `AspNetRoles`, etc.).
- Se não houver migrações no repositório, crie uma migração e aplique (apenas se necessário):

```bash
# criar migração (gera arquivos em Migrations/ para IdentityContext)
dotnet ef migrations add InitialIdentityCreate --context IdentityContext
# aplicar
dotnet ef database update --context IdentityContext
```

---

## 7) Aplicar migrações / criar esquema do banco principal (Aluguelink)
Se o banco `aluguelink` já existe e contém esquema (por exemplo vindo de um .mwb), talvez não seja necessário aplicar migrações; caso queira garantir que as tabelas usadas pelo projeto existam, execute:

```bash
# aplicar migrações para o contexto principal
dotnet ef database update --context AluguelinkContext
```

Se não houver migrações no repositório, pode ser preciso gerar uma migração `InitialAluguelinkCreate` e rodá-la (opcional).

---

## 8) Build e execução
```bash
# a partir da pasta AlugueLinkWEB
dotnet build
dotnet run
```
A aplicação abrirá em `https://localhost:5001` (ou `http://localhost:5000`).

---

## 9) Testes rápidos
- Abra o browser em `https://localhost:5001`.
- Na landing page: clique em **Criar Conta Grátis** e registre um usuário.
- Faça login com o usuário criado.
- Verifique que a sidebar aparece e o dashboard funciona.

---

## 10) Observações sobre SMTP / envio de email
- O arquivo `appsettings.json` inclui seção `Smtp` com credenciais para envio de email. No repositório essas credenciais são placeholders (`no-reply@aluguelink.com`, `change_me`).
- Para desenvolvimento, não é obrigatório configurar SMTP — o sistema aceita registros sem confirmação se a aplicação estiver configurada assim (ver `Program.cs`).
- Se quiser ativar envio real, use credenciais reais (ou variáveis de ambiente / user-secrets) e atualize `appsettings.json`.

---

## 11) Solução de problemas comuns
- Erro de conexão MySQL: verifique se o serviço MySQL está rodando e credenciais em `appsettings.json`.
- Comando `dotnet ef` não encontrado: instale `dotnet-ef` globalmente (veja seção 1).
- Migration exception: confira permissões do usuário MySQL (GRANT) e charset/collation do banco.

---

## 12) Resumo dos comandos (rápido)
```bash
# 1. Restore
dotnet restore

# 2. Criar banco Identity (se necessário) - no MySQL
# CREATE DATABASE IdentityUsers ...

# 3. Aplicar migrações Identity
cd AlugueLinkWEB
dotnet ef database update --context IdentityContext

# 4. Aplicar migrações do banco principal (opcional)
dotnet ef database update --context AluguelinkContext

# 5. Build e run
dotnet build
dotnet run
```

---


