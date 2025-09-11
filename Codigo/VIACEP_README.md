# Integração ViaCEP - AlugueLink

## Visão Geral

Este documento descreve a implementação da integração com a API pública ViaCEP para preenchimento automático de endereços no sistema AlugueLink.

## Componentes Implementados

### 1. Backend (.NET 8)

#### Core/DTO/ViaCepDTO.cs
- DTO que representa a resposta da API ViaCEP
- Contém todos os campos retornados pela API: CEP, logradouro, complemento, bairro, localidade (cidade), UF (estado), IBGE, GIA, DDD e SIAFI

#### Core/Service/IViaCepService.cs
- Interface do serviço ViaCEP
- Define o método `BuscarEnderecoPorCepAsync(string cep)`

#### Service/ViaCepService.cs
- Implementação do serviço ViaCEP
- Consume a API `https://viacep.com.br/ws/{CEP}/json/`
- Inclui validação de CEP, tratamento de erros e limpeza de dados
- Trata erros de rede, CEPs inválidos e respostas inválidas

#### AlugueLinkWEB/Controllers/ViaCepController.cs
- API Controller que expõe endpoint `/api/viacep/{cep}`
- Permite acesso anônimo para consulta de endereços
- Retorna dados formatados em JSON

### 2. Frontend (JavaScript/CSS)

#### AlugueLinkWEB/wwwroot/js/viacep-util.js
- Utilitário JavaScript para integração com ViaCEP
- Funcionalidades:
  - Validação de CEP em tempo real
  - Formatação automática (00000-000)
  - Preenchimento automático dos campos de endereço
  - Estados de loading e erro
  - API pública para uso programático

#### AlugueLinkWEB/wwwroot/css/viacep.css
- Estilos CSS para a integração
- Estados visuais de loading, erro e sucesso
- Animações para preenchimento de campos
- Design responsivo

### 3. Helpers e Utilitários

#### AlugueLinkWEB/Helpers/ViaCepHelper.cs
- Métodos utilitários para validação e formatação de CEP no backend
- Geração de scripts JavaScript para inicialização
- Validação de CEP brasileiro

## Como Usar

### 1. Em Formulários de Cadastro

A integração foi implementada automaticamente nos seguintes formulários:

- **Cadastro de Imóveis** (`/Imovel/Create` e `/Imovel/Edit`)
- **Cadastro de Inquilinos** (`/Locatario/Create` e `/Locatario/Edit`)

### 2. Para Usar em Novos Formulários

#### No HTML/Razor:
```html
<!-- Campo CEP -->
<input asp-for="Cep" class="form-control" id="Cep" placeholder="00000-000" />
<div id="cep-error-message" class="text-danger small" style="display: none;"></div>

<!-- Campos de endereço -->
<input asp-for="Logradouro" class="form-control" id="Logradouro" />
<input asp-for="Complemento" class="form-control" id="Complemento" />
<input asp-for="Bairro" class="form-control" id="Bairro" />
<input asp-for="Cidade" class="form-control" id="Cidade" />
<select asp-for="Estado" class="form-select" id="Estado">
    <!-- Opções de estados -->
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
Busca informações de endereço por CEP.

**Parâmetros:**
- `cep` (string): CEP brasileiro (com ou sem máscara)

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
  "localidade": "São Paulo",
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
  "mensagem": "CEP não encontrado."
}
```

## Funcionalidades

### ? Implementadas

1. **Validação de CEP**: Verifica formato brasileiro (8 dígitos)
2. **Formatação Automática**: Aplica máscara 00000-000 durante digitação
3. **Preenchimento Automático**: Preenche campos de endereço automaticamente
4. **Tratamento de Erros**: Exibe mensagens de erro apropriadas
5. **Estados Visuais**: Loading, erro e sucesso
6. **Limpeza de Dados**: Remove caracteres não numéricos
7. **Reutilização**: Pode ser usado em qualquer formulário
8. **Testes Unitários**: Cobertura de testes para serviços e helpers
9. **Design Responsivo**: Funciona em dispositivos móveis

### ?? Comportamento da Interface

1. **Digitação**: CEP é formatado automaticamente durante a digitação
2. **Blur/Saída do Campo**: Dispara consulta à API quando o usuário sai do campo
3. **Loading**: Campos ficam em modo somente leitura com indicador visual
4. **Sucesso**: Campos são preenchidos com animação
5. **Erro**: Campo CEP é marcado como inválido e mensagem é exibida
6. **Limpeza**: Erros são limpos quando o usuário volta a digitar

## Tratamento de Erros

### Backend
- CEP inválido (formato incorreto)
- CEP não encontrado
- Falhas de rede/timeout
- Resposta inválida da API
- Erros internos do servidor

### Frontend
- Validação de formato em tempo real
- Mensagens de erro contextuais
- Recuperação automática de erros
- Fallback para entrada manual

## Configurações

### HttpClient
O serviço utiliza HttpClient registrado via DI no `Program.cs`:
```csharp
builder.Services.AddHttpClient<IViaCepService, ViaCepService>();
```

### Timeout
O timeout padrão do HttpClient é usado (100 segundos). Para personalizar:
```csharp
builder.Services.AddHttpClient<IViaCepService, ViaCepService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});
```

## Testes

### Testes Unitários
- `ServiceTests/ViaCepServiceTests.cs`: Testa o serviço backend
- `AlugueLinkWebTests/ViaCepHelperTests.cs`: Testa os helpers

### Executar Testes
```bash
dotnet test
```

## Dependências

### Backend
- .NET 8
- System.Text.Json (para serialização)
- HttpClient (para requisições HTTP)

### Frontend
- Vanilla JavaScript (ES6+)
- CSS3 (para animações)
- Bootstrap (para estilos base)

## Manutenção

### Logs
Erros são registrados no console do navegador para depuração.

### Monitoring
Considere implementar:
- Logs estruturados no backend
- Métricas de uso da API
- Monitoramento de performance

## Limitações Conhecidas

1. **Rate Limiting**: A API ViaCEP pode ter limites de requisições
2. **Disponibilidade**: Dependente da disponibilidade da API externa
3. **CEPs Novos**: Pode não conter CEPs muito recentes
4. **Conexão**: Requer conexão com internet

## Exemplo de Uso Completo

Veja os arquivos de exemplo implementados:
- `Views/Imovel/Create.cshtml`
- `Views/Imovel/Edit.cshtml`
- `Views/Locatario/Create.cshtml`
- `Views/Locatario/Edit.cshtml`

Para mais informações sobre a API ViaCEP, consulte: https://viacep.com.br/