using System.ComponentModel.DataAnnotations;

namespace Util
{
    public class CPFAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return true;
            
            var valueNoEspecial = Methods.RemoveSpecialsCaracts((string)value);
            var valido = Methods.ValidarCpf(valueNoEspecial);
            return valido;
        }

        public override string FormatErrorMessage(string name)
        {
            return "CPF Inválido";
        }
    }
}