using System.Text.RegularExpressions;

namespace Util
{
    /// <summary>
    /// Métodos utilitários para validações e formatações
    /// </summary>
    public static class Methods
    {
        /// <summary>
        /// Remove caracteres especiais de uma string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveSpecialsCaracts(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            
            // Remove pontos, traços, barras e outros caracteres especiais comuns
            return text.Replace(".", "").Replace("-", "").Replace("/", "").Replace("(", "")
                      .Replace(")", "").Replace(" ", "").Replace(",", "");
        }

        /// <summary>
        /// Formatar CPF para o padrão brasileiro (000.000.000-00)
        /// </summary>
        /// <param name="cpf"></param>
        /// <returns></returns>
        public static string PatternCpf(string cpf)
        {
            if (string.IsNullOrEmpty(cpf) || cpf.Length != 11)
                return cpf;
            
            return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
        }

        /// <summary>
        /// Formatar CNPJ para o padrão brasileiro (00.000.000/0000-00)
        /// </summary>
        /// <param name="cnpj"></param>
        /// <returns></returns>
        public static string PatternCnpj(string cnpj)
        {
            if (string.IsNullOrEmpty(cnpj) || cnpj.Length != 14)
                return cnpj;
            
            return $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";
        }

        /// <summary>
        /// Remove caracteres não numéricos
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveNaoNumericos(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            
            Regex reg = new Regex(@"[^0-9]");
            return reg.Replace(text, string.Empty);
        }

        /// <summary>
        /// Validar CPF brasileiro
        /// </summary>
        /// <param name="cpf"></param>
        /// <returns></returns>
        public static bool ValidarCpf(string cpf)
        {
            cpf = RemoveNaoNumericos(cpf);

            if (string.IsNullOrEmpty(cpf) || cpf.Length != 11)
                return false;

            // Verifica se todos os dígitos são iguais (CPFs inválidos conhecidos)
            if (cpf.All(c => c == cpf[0]))
                return false;

            var multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            
            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            string digito = resto.ToString();
            tempCpf += digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            
            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;
            digito += resto.ToString();

            return cpf.EndsWith(digito);
        }

        /// <summary>
        /// Validar CNPJ brasileiro
        /// </summary>
        /// <param name="cnpj"></param>
        /// <returns></returns>
        public static bool ValidarCnpj(string cnpj)
        {
            cnpj = RemoveNaoNumericos(cnpj);

            if (string.IsNullOrEmpty(cnpj) || cnpj.Length != 14)
                return false;

            // Verifica se todos os dígitos são iguais (CNPJs inválidos conhecidos)
            if (cnpj.All(c => c == cnpj[0]))
                return false;

            int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCnpj = cnpj.Substring(0, 12);
            int soma = 0;

            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            string digito = resto.ToString();
            tempCnpj += digito;
            soma = 0;

            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;
            digito += resto.ToString();

            return cnpj.EndsWith(digito);
        }

        /// <summary>
        /// Verifica se o texto contém apenas números
        /// </summary>
        /// <param name="texto"></param>
        /// <returns></returns>
        public static bool SoContemNumeros(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return false;
            
            texto = RemoveSpecialsCaracts(texto);
            return Regex.IsMatch(texto, "^[0-9]*$");
        }

        /// <summary>
        /// Verifica se o texto contém apenas letras e espaços
        /// </summary>
        /// <param name="texto"></param>
        /// <returns></returns>
        public static bool SoContemLetras(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return false;
            
            return Regex.IsMatch(texto, @"^[a-zA-ZÀ-ÿ\s]*$");
        }

        /// <summary>
        /// Formatar CEP para o padrão brasileiro (00000-000)
        /// </summary>
        /// <param name="cep"></param>
        /// <returns></returns>
        public static string PatternCep(string cep)
        {
            if (string.IsNullOrEmpty(cep) || cep.Length != 8)
                return cep;
            
            return $"{cep.Substring(0, 5)}-{cep.Substring(5, 3)}";
        }

        /// <summary>
        /// Formatar telefone para o padrão brasileiro (00) 00000-0000
        /// </summary>
        /// <param name="telefone"></param>
        /// <returns></returns>
        public static string PatternTelefone(string telefone)
        {
            if (string.IsNullOrEmpty(telefone))
                return telefone;
            
            telefone = RemoveNaoNumericos(telefone);
            
            if (telefone.Length == 11) // Celular
                return $"({telefone.Substring(0, 2)}) {telefone.Substring(2, 5)}-{telefone.Substring(7, 4)}";
            else if (telefone.Length == 10) // Fixo
                return $"({telefone.Substring(0, 2)}) {telefone.Substring(2, 4)}-{telefone.Substring(6, 4)}";
            
            return telefone;
        }

        /// <summary>
        /// Validar email simples
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool ValidarEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;
            
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
}