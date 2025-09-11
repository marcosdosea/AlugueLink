# Integra��o ViaCEP - AlugueLink

## Vis�o Geral

Este documento descreve a implementa��o da integra��o com a API p�blica ViaCEP para preenchimento autom�tico de endere�os no sistema AlugueLink.

## Componentes Implementados

### 1. Backend (.NET 8)

#### Core/DTO/ViaCepDTO.cs
- DTO que representa a resposta da API ViaCEP
- Cont�m todos os campos retornados pela API: CEP, logradouro, complemento, bairro, localidade (cidade), UF (estado), IBGE, GIA, DDD e SIAFI

#### Core/Service/IViaCepService.cs
- Interface do servi�o ViaCEP
- Define o m�todo `BuscarEnderecoPorCepAsync(string cep)`

#### Service/ViaCepService.cs
- Implementa��o do servi�o ViaCEP
- Consume a API `https://viacep.com.br/ws/{CEP}/json/`
- Inclui valida��o de CEP, tratamento de erros e limpeza de dados
- Trata erros de rede, CEPs inv�lidos e respostas inv�lidas

#### AlugueLinkWEB/Controllers/ViaCepController.cs
- API Controller que exp�e endpoint `/api/viacep/{cep}`
- Permite acesso an�nimo para consulta de endere�os
- Retorna dados formatados em JSON

### 2. Frontend (JavaScript/CSS)

#### AlugueLinkWEB/wwwroot/js/viacep-util.js
- Utilit�rio JavaScript para integra��o com ViaCEP
- Funcionalidades:
  - Valida��o de CEP em tempo real
  - Formata��o autom�tica (00000-000)
  - Preenchimento autom�tico dos campos de endere�o
  - Estados de loading e erro
  - API p�blica para uso program�tico

#### AlugueLinkWEB/wwwroot/css/viacep.css
- Estilos CSS para a integra��o
- Estados visuais de loading, erro e sucesso
- Anima��es para preenchimento de campos
- Design responsivo

### 3. Helpers e Utilit�rios

#### AlugueLinkWEB/Helpers/ViaCepHelper.cs
- M�todos utilit�rios para valida��o e formata��o de CEP no backend
- Gera��o de scripts JavaScript para inicializa��o
- Valida��o de CEP brasileiro

## Como Usar

### 1. Em Formul�rios de Cadastro

A integra��o foi implementada automaticamente nos seguintes formul�rios:

- **Cadastro de Im�veis** (`/Imovel/Create` e `/Imovel/Edit`)
- **Cadastro de Inquilinos** (`/Locatario/Create` e `/Locatario/Edit`)

### 2. Para Usar em Novos Formul�rios

#### No HTML/Razor:
```html
<!-- Campo CEP -->
<input asp-for="Cep" class="form-control" id="Cep" placeholder="00000-000" />
<div id="cep-error-message" class="text-danger small" style="display: none;"></div>

<!-- Campos de endere�o -->
<input asp-for="Logradouro" class="form-control" id="Logradouro" />
<input asp-for="Complemento" class="form-control" id="Complemento" />
<input asp-for="Bairro" class="form-control" id="Bairro" />
<input asp-for="Cidade" class="form-control" id="Cidade" />
<select asp-for="Estado" class="form-select" id="Estado">
    <!-- Op��es de estados -->
</select>
```

#### No JavaScript:
```javascript
@section Scripts {
    <script src="~/js/viacep-util.js"></script>
    <script>
        document.addEventListener('DOMContentLoaded', function() {
            ViaCepUtil.init({
                cep: 'Cep',
                logradouro: 'Logradouro',
                complemento: 'Complemento',
                bairro: 'Bairro',
                cidade: 'Cidade',
                estado: 'Estado'
            });
        });
    </script>
}
```

### 3. API Endpoints

#### GET /api/viacep/{cep}
Busca informa��es de endere�o por CEP.

**Par�metros:**
- `cep` (string): CEP brasileiro (com ou sem m�scara)

**Exemplo de Request:**
```
GET /api/viacep/01310100
```

**Exemplo de Response:**
```json
{
  "cep": "01310-100",
  "logradouro": "Avenida Paulista",
  "complemento": "",
  "bairro": "Bela Vista",
  "localidade": "S�o Paulo",
  "uf": "SP",
  "ibge": "3550308",
  "gia": "1004",
  "ddd": "11",
  "siafi": "7107"
}
```

**Exemplo de Error Response:**
```json
{
  "erro": true,
  "mensagem": "CEP n�o encontrado."
}
```

## Funcionalidades

### ? Implementadas

1. **Valida��o de CEP**: Verifica formato brasileiro (8 d�gitos)
2. **Formata��o Autom�tica**: Aplica m�scara 00000-000 durante digita��o
3. **Preenchimento Autom�tico**: Preenche campos de endere�o automaticamente
4. **Tratamento de Erros**: Exibe mensagens de erro apropriadas
5. **Estados Visuais**: Loading, erro e sucesso
6. **Limpeza de Dados**: Remove caracteres n�o num�ricos
7. **Reutiliza��o**: Pode ser usado em qualquer formul�rio
8. **Testes Unit�rios**: Cobertura de testes para servi�os e helpers
9. **Design Responsivo**: Funciona em dispositivos m�veis

### ?? Comportamento da Interface

1. **Digita��o**: CEP � formatado automaticamente durante a digita��o
2. **Blur/Sa�da do Campo**: Dispara consulta � API quando o usu�rio sai do campo
3. **Loading**: Campos ficam em modo somente leitura com indicador visual
4. **Sucesso**: Campos s�o preenchidos com anima��o
5. **Erro**: Campo CEP � marcado como inv�lido e mensagem � exibida
6. **Limpeza**: Erros s�o limpos quando o usu�rio volta a digitar

## Tratamento de Erros

### Backend
- CEP inv�lido (formato incorreto)
- CEP n�o encontrado
- Falhas de rede/timeout
- Resposta inv�lida da API
- Erros internos do servidor

### Frontend
- Valida��o de formato em tempo real
- Mensagens de erro contextuais
- Recupera��o autom�tica de erros
- Fallback para entrada manual

## Configura��es

### HttpClient
O servi�o utiliza HttpClient registrado via DI no `Program.cs`:
```csharp
builder.Services.AddHttpClient<IViaCepService, ViaCepService>();
```

### Timeout
O timeout padr�o do HttpClient � usado (100 segundos). Para personalizar:
```csharp
builder.Services.AddHttpClient<IViaCepService, ViaCepService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});
```

## Testes

### Testes Unit�rios
- `ServiceTests/ViaCepServiceTests.cs`: Testa o servi�o backend
- `AlugueLinkWebTests/ViaCepHelperTests.cs`: Testa os helpers

### Executar Testes
```bash
dotnet test
```

## Depend�ncias

### Backend
- .NET 8
- System.Text.Json (para serializa��o)
- HttpClient (para requisi��es HTTP)

### Frontend
- Vanilla JavaScript (ES6+)
- CSS3 (para anima��es)
- Bootstrap (para estilos base)

## Manuten��o

### Logs
Erros s�o registrados no console do navegador para depura��o.

### Monitoring
Considere implementar:
- Logs estruturados no backend
- M�tricas de uso da API
- Monitoramento de performance

## Limita��es Conhecidas

1. **Rate Limiting**: A API ViaCEP pode ter limites de requisi��es
2. **Disponibilidade**: Dependente da disponibilidade da API externa
3. **CEPs Novos**: Pode n�o conter CEPs muito recentes
4. **Conex�o**: Requer conex�o com internet

## Exemplo de Uso Completo

Veja os arquivos de exemplo implementados:
- `Views/Imovel/Create.cshtml`
- `Views/Imovel/Edit.cshtml`
- `Views/Locatario/Create.cshtml`
- `Views/Locatario/Edit.cshtml`

Para mais informa��es sobre a API ViaCEP, consulte: https://viacep.com.br/