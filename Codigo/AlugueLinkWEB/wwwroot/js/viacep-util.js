// Utilitário para integração com a API ViaCEP
window.ViaCepUtil = (function() {
    'use strict';

    // Configuração
    const CONFIG = {
        apiBaseUrl: '/api/viacep/',
        loadingText: 'Buscando...',
        errorClass: 'is-invalid',
        loadingClass: 'loading-address'
    };

    // Elementos do formulário (serão definidos dinamicamente)
    let elements = {};

    /**
     * Inicializa o utilitário ViaCEP
     * @param {Object} elementIds - IDs dos elementos do formulário
     */
    function init(elementIds) {
        elements = {
            cep: document.getElementById(elementIds.cep || 'Cep'),
            logradouro: document.getElementById(elementIds.logradouro || 'Logradouro'),
            complemento: document.getElementById(elementIds.complemento || 'Complemento'),
            bairro: document.getElementById(elementIds.bairro || 'Bairro'),
            cidade: document.getElementById(elementIds.cidade || 'Cidade'),
            estado: document.getElementById(elementIds.estado || 'Estado')
        };

        if (elements.cep) {
            // Adicionar event listeners
            elements.cep.addEventListener('blur', handleCepBlur);
            elements.cep.addEventListener('input', handleCepInput);
        }

        console.log('ViaCepUtil inicializado');
    }

    /**
     * Limpa o CEP, removendo caracteres não numéricos
     */
    function limparCep(cep) {
        return cep ? cep.replace(/\D/g, '') : '';
    }

    /**
     * Valida se o CEP tem o formato correto
     */
    function validarCep(cep) {
        const cepLimpo = limparCep(cep);
        return cepLimpo.length === 8 && /^[0-9]{8}$/.test(cepLimpo);
    }

    /**
     * Formata o CEP para exibição (00000-000)
     */
    function formatarCep(cep) {
        const cepLimpo = limparCep(cep);
        if (cepLimpo.length === 8) {
            return cepLimpo.replace(/^(\d{5})(\d{3})$/, '$1-$2');
        }
        return cep;
    }

    /**
     * Limpa os campos de endereço
     */
    function limparCamposEndereco() {
        if (elements.logradouro) elements.logradouro.value = '';
        if (elements.complemento) elements.complemento.value = '';
        if (elements.bairro) elements.bairro.value = '';
        if (elements.cidade) elements.cidade.value = '';
        if (elements.estado) elements.estado.value = '';
    }

    /**
     * Preenche os campos de endereço com os dados retornados
     */
    function preencherCamposEndereco(dados) {
        if (elements.logradouro && dados.logradouro) {
            elements.logradouro.value = dados.logradouro;
        }
        
        if (elements.complemento && dados.complemento) {
            elements.complemento.value = dados.complemento;
        }
        
        if (elements.bairro && dados.bairro) {
            elements.bairro.value = dados.bairro;
        }
        
        if (elements.cidade && dados.localidade) {
            elements.cidade.value = dados.localidade;
        }
        
        if (elements.estado && dados.uf) {
            elements.estado.value = dados.uf;
        }
    }

    /**
     * Define o estado de loading nos campos
     */
    function setLoadingState(loading) {
        const campos = [elements.logradouro, elements.bairro, elements.cidade, elements.estado];
        
        campos.forEach(campo => {
            if (campo) {
                if (loading) {
                    campo.classList.add(CONFIG.loadingClass);
                    campo.setAttribute('readonly', 'readonly');
                    if (campo !== elements.estado) {
                        campo.value = CONFIG.loadingText;
                    }
                } else {
                    campo.classList.remove(CONFIG.loadingClass);
                    campo.removeAttribute('readonly');
                }
            }
        });
    }

    /**
     * Remove classes de erro dos campos
     */
    function limparErros() {
        Object.values(elements).forEach(element => {
            if (element) {
                element.classList.remove(CONFIG.errorClass);
            }
        });
    }

    /**
     * Adiciona classe de erro ao campo CEP
     */
    function marcarErroInput() {
        if (elements.cep) {
            elements.cep.classList.add(CONFIG.errorClass);
        }
    }

    /**
     * Exibe mensagem de erro
     */
    function exibirErro(mensagem) {
        // Pode ser customizado para usar toast, modal, etc.
        console.warn('Erro ViaCEP:', mensagem);
        
        // Procura por elemento para exibir erro
        const errorElement = document.getElementById('cep-error-message');
        if (errorElement) {
            errorElement.textContent = mensagem;
            errorElement.style.display = 'block';
            
            // Esconder mensagem após 5 segundos
            setTimeout(() => {
                errorElement.style.display = 'none';
            }, 5000);
        } else {
            // Fallback: alerta simples
            alert(mensagem);
        }
    }

    /**
     * Busca o endereço via API
     */
    async function buscarEndereco(cep) {
        try {
            const cepLimpo = limparCep(cep);
            const response = await fetch(`${CONFIG.apiBaseUrl}${cepLimpo}`);
            
            if (!response.ok) {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }

            const dados = await response.json();
            
            if (dados.erro) {
                throw new Error('CEP não encontrado');
            }

            return dados;
        } catch (error) {
            console.error('Erro ao buscar CEP:', error);
            throw error;
        }
    }

    /**
     * Handler para o evento blur do campo CEP
     */
    async function handleCepBlur(event) {
        const cep = event.target.value;
        
        if (!cep) {
            return;
        }

        // Validar CEP
        if (!validarCep(cep)) {
            marcarErroInput();
            exibirErro('CEP inválido. Digite um CEP válido com 8 dígitos.');
            return;
        }

        // Limpar erros anteriores
        limparErros();
        
        // Iniciar loading
        setLoadingState(true);

        try {
            const dadosEndereco = await buscarEndereco(cep);
            preencherCamposEndereco(dadosEndereco);
            
            // Formatar CEP no campo
            elements.cep.value = formatarCep(cep);
            
        } catch (error) {
            marcarErroInput();
            limparCamposEndereco();
            exibirErro(error.message || 'Erro ao buscar CEP. Tente novamente.');
        } finally {
            setLoadingState(false);
        }
    }

    /**
     * Handler para o evento input do campo CEP (formatação em tempo real)
     */
    function handleCepInput(event) {
        const cep = event.target.value;
        const cepLimpo = limparCep(cep);
        
        // Limitar a 8 dígitos
        if (cepLimpo.length <= 8) {
            // Formatar durante a digitação
            if (cepLimpo.length > 5) {
                event.target.value = cepLimpo.replace(/^(\d{5})(\d{0,3})$/, '$1-$2');
            } else {
                event.target.value = cepLimpo;
            }
        } else {
            // Manter apenas os primeiros 8 dígitos
            const cepTruncado = cepLimpo.substring(0, 8);
            event.target.value = formatarCep(cepTruncado);
        }

        // Limpar erros quando o usuário começar a digitar novamente
        if (elements.cep.classList.contains(CONFIG.errorClass)) {
            elements.cep.classList.remove(CONFIG.errorClass);
        }
    }

    /**
     * Função pública para buscar CEP programaticamente
     */
    async function buscarCepProgramaticamente(cep) {
        if (!validarCep(cep)) {
            throw new Error('CEP inválido');
        }

        return await buscarEndereco(cep);
    }

    // API pública
    return {
        init: init,
        buscarCep: buscarCepProgramaticamente,
        validarCep: validarCep,
        limparCep: limparCep,
        formatarCep: formatarCep
    };
})();