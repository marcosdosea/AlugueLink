# Instruções de configuração e execução — AlugueLinkWEB

Este documento descreve os passos mínimos para configurar e executar este repositório localmente. O objetivo é deixar o projeto pronto para um novo usuário com o menor número de passos possíveis.

Premissas e convenções usadas neste repositório:
- O script SQL completo de criação do banco está em: sql/scriptDatabase_Aluguelink.sql — ele cria o schema `aluguelink` e também o schema `IdentityUsers` com as tabelas do ASP.NET Identity prontas para uso.
- A configuração padrão usa MySQL em `localhost:3306` com usuário `root` e senha `123456`. Se o seu ambiente for diferente, ajuste `AlugueLinkWEB/appsettings.json` antes de executar.
- **Este projeto NÃO usa migrations** - apenas o script SQL como fonte única da verdade do banco.

---

## 1) Pré-requisitos
- .NET 8 SDK: https://dotnet.microsoft.com/download
- MySQL Server (rodando localmente) e cliente (MySQL Workbench recomendado)

---

## 2) Preparar o banco (passo único obrigatório)
1. Abra o MySQL Workbench (ou outro cliente) conectado ao seu servidor MySQL (padrão: localhost:3306, usuário: root, senha: 123456).
2. Abra o arquivo `sql/scriptDatabase_Aluguelink.sql` e execute-o. Este script irá:
   - Criar o schema `aluguelink` e todas as tabelas necessárias pela aplicação (locador, imovel, locatario, aluguel, manutencao, pagamento, etc.).
   - Criar o schema `IdentityUsers` com as tabelas do ASP.NET Identity completas.
   - Inserir dados iniciais necessários e deixar o banco pronto para uso.

**?? IMPORTANTE: Este é o único método suportado para criação do banco. Migrations foram removidas do projeto.**

---

## 3) Ajustar connection strings (se necessário)
Abra `AlugueLinkWEB/appsettings.json` e verifique as connection strings:

```json
"ConnectionStrings": {
  "AluguelinkDatabase": "server=localhost;port=3306;user=root;password=123456;database=aluguelink",
  "IdentityDatabase": "server=localhost;port=3306;user=root;password=123456;database=IdentityUsers"
}
```

Altere `user`/`password`/`server` se o seu MySQL não usar as credenciais padrão.

---

## 4) Build e execução
No terminal, a partir da pasta raiz do repositório (`Codigo`):

```bash
cd AlugueLinkWEB
dotnet restore
dotnet build
dotnet run
```

A aplicação estará disponível em `https://localhost:5001` (ou `http://localhost:5000`).

---

## 5) Acesso inicial
- Um usuário administrador é criado automaticamente durante a inicialização da aplicação (se não existir).
- Credenciais padrão configuradas em `appsettings.json` para facilitar testes:
  - Email: `admin@aluguelink.com`
  - Senha: `Admin1234!`

Se desejar alterar essas credenciais, edite a seção `AdminUser` em `AlugueLinkWEB/appsettings.json` antes de rodar a aplicação.

---

## Solução de problemas rápidos
- Erro de conexão MySQL: verifique se o serviço MySQL está rodando e as credenciais em `appsettings.json`.
- Script SQL falha com erros de permissão: verifique se o usuário MySQL tem permissões para criar bases e tabelas (GRANT).
- Usuário admin não aparece: confira os logs da aplicação; o seeding de dados tenta criar a role `Administrator` e o usuário admin se necessário.

---

## Resumo rápido (fluxo obrigatório)
1. Executar `sql/scriptDatabase_Aluguelink.sql` no MySQL Workbench (usuário root / senha 123456)
2. cd AlugueLinkWEB
3. dotnet restore
4. dotnet build
5. dotnet run

**Nota:** Migrations foram removidas - use apenas o script SQL.




