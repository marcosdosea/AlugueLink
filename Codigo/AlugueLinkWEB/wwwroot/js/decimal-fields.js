/**
 * Funcionalidades para campos decimais
 * Utilizado em campos de valor monetário e área
 */

document.addEventListener('DOMContentLoaded', function() {
    // Selecionar todos os campos com classe decimal-field
    const decimalFields = document.querySelectorAll('.decimal-field');
    
    decimalFields.forEach(field => {
        // Formatar valor inicial se existir
        if (field.value) {
            field.value = formatDecimalValue(field.value);
        }
        
        // Adicionar evento de entrada (keyup)
        field.addEventListener('keyup', function(e) {
            formatDecimalInput(e.target);
        });
        
        // Adicionar evento de perda de foco
        field.addEventListener('blur', function(e) {
            validateDecimalField(e.target);
        });
    });
});

/**
 * Formatar valor decimal para exibição
 * @param {string} value - Valor a ser formatado
 * @returns {string} Valor formatado
 */
function formatDecimalValue(value) {
    if (!value) return '';
    
    // Remover caracteres não numéricos exceto vírgula
    let cleaned = value.toString().replace(/[^\d,]/g, '');
    
    // Garantir apenas uma vírgula
    const parts = cleaned.split(',');
    if (parts.length > 2) {
        cleaned = parts[0] + ',' + parts.slice(1).join('');
    }
    
    return cleaned;
}

/**
 * Formatar entrada do usuário em tempo real
 * @param {HTMLElement} field - Campo de entrada
 */
function formatDecimalInput(field) {
    const cursorPosition = field.selectionStart;
    const originalValue = field.value;
    
    // Formatar valor
    const formattedValue = formatDecimalValue(originalValue);
    
    if (formattedValue !== originalValue) {
        field.value = formattedValue;
        
        // Restaurar posição do cursor
        const newPosition = cursorPosition + (formattedValue.length - originalValue.length);
        field.setSelectionRange(newPosition, newPosition);
    }
}

/**
 * Validar campo decimal
 * @param {HTMLElement} field - Campo a ser validado
 */
function validateDecimalField(field) {
    const value = field.value.trim();
    const isValid = /^\d+,\d{1,2}$/.test(value) || value === '';
    
    // Remover classes de validação anteriores
    field.classList.remove('is-valid', 'is-invalid');
    
    if (value !== '') {
        if (isValid) {
            field.classList.add('is-valid');
        } else {
            field.classList.add('is-invalid');
        }
    }
    
    return isValid;
}

/**
 * Converter valor decimal para number
 * @param {string} decimalString - String no formato brasileiro (1.234,56)
 * @returns {number} Valor numérico
 */
function parseDecimalValue(decimalString) {
    if (!decimalString) return 0;
    
    return parseFloat(decimalString.replace(',', '.'));
}

/**
 * Converter number para formato decimal brasileiro
 * @param {number} number - Número a ser convertido
 * @param {number} decimals - Número de casas decimais (padrão: 2)
 * @returns {string} String formatada
 */
function toDecimalString(number, decimals = 2) {
    if (isNaN(number)) return '';
    
    return number.toFixed(decimals).replace('.', ',');
}