using System.ComponentModel.DataAnnotations;

namespace Util
{
    /// <summary>
    /// Valida��o customizada para CPF
    /// </summary>
    public class CPFAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return true;
            
            var valueNoEspecial = Methods.RemoveSpecialsCaracts((string)value);
            bool valido = Methods.ValidarCpf(valueNoEspecial);
            return valido;
        }

        public override string FormatErrorMessage(string name)
        {
            return "CPF Inv�lido";
        }
    }
}