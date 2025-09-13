using System.Text.RegularExpressions;

namespace Util
{
    public static class Methods
    {
        public static string RemoveSpecialsCaracts(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            
            return text.Replace(".", "").Replace("-", "").Replace("/", "").Replace("(", "")
                      .Replace(")", "").Replace(" ", "").Replace(",", "");
        }

        public static string PatternCpf(string cpf)
        {
            if (string.IsNullOrEmpty(cpf) || cpf.Length != 11)
                return cpf;
            
            return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
        }

        public static string RemoveNaoNumericos(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            
            var reg = new Regex(@"[^0-9]");
            return reg.Replace(text, string.Empty);
        }

        public static bool ValidarCpf(string cpf)
        {
            cpf = RemoveNaoNumericos(cpf);

            if (string.IsNullOrEmpty(cpf) || cpf.Length != 11)
                return false;

            if (cpf.All(c => c == cpf[0]))
                return false;

            var multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            
            var tempCpf = cpf.Substring(0, 9);
            var soma = 0;

            for (var i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            var resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            var digito = resto.ToString();
            tempCpf += digito;
            soma = 0;

            for (var i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            
            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;
            digito += resto.ToString();

            return cpf.EndsWith(digito);
        }

        public static bool SoContemNumeros(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return false;
            
            texto = RemoveSpecialsCaracts(texto);
            return Regex.IsMatch(texto, "^[0-9]*$");
        }

        public static bool SoContemLetras(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return false;
            
            return Regex.IsMatch(texto, @"^[a-zA-Z\u00C0-\u017F\s]*$");
        }

        public static string PatternCep(string cep)
        {
            if (string.IsNullOrEmpty(cep) || cep.Length != 8)
                return cep;
            
            return $"{cep.Substring(0, 5)}-{cep.Substring(5, 3)}";
        }

        public static string PatternTelefone(string telefone)
        {
            if (string.IsNullOrEmpty(telefone))
                return telefone;
            
            telefone = RemoveNaoNumericos(telefone);
            
            if (telefone.Length == 11)
                return $"({telefone.Substring(0, 2)}) {telefone.Substring(2, 5)}-{telefone.Substring(7, 4)}";
            else if (telefone.Length == 10)
                return $"({telefone.Substring(0, 2)}) {telefone.Substring(2, 4)}-{telefone.Substring(6, 4)}";
            
            return telefone;
        }

        public static bool ValidarEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;
            
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
}