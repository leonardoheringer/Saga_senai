// Validador de CPF - Sistema Petrobras
// Validação em tempo real com formatação automática

class CPFValidator {
    constructor() {
        this.initializeCPFFields();
    }
    
    // Inicializar campos de CPF
    initializeCPFFields() {
        document.addEventListener('DOMContentLoaded', () => {
            const cpfFields = document.querySelectorAll('input[name*="CPF"], input[data-cpf], .cpf-input');
            
            cpfFields.forEach(field => {
                // Formatação em tempo real
                field.addEventListener('input', (e) => {
                    this.formatCPF(e.target);
                    this.validateCPFField(e.target);
                });
                
                // Validação ao perder foco
                field.addEventListener('blur', (e) => {
                    this.validateCPFField(e.target);
                });
                
                // Permitir apenas números
                field.addEventListener('keypress', (e) => {
                    if (!/\d/.test(e.key) && !['Backspace', 'Delete', 'Tab', 'Enter', 'ArrowLeft', 'ArrowRight'].includes(e.key)) {
                        e.preventDefault();
                    }
                });
            });
        });
    }
    
    // Formatar CPF (000.000.000-00)
    formatCPF(input) {
        let value = input.value.replace(/\D/g, '');
        
        // Limitar a 11 dígitos
        if (value.length > 11) {
            value = value.substring(0, 11);
        }
        
        // Aplicar formatação
        if (value.length > 9) {
            value = value.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
        } else if (value.length > 6) {
            value = value.replace(/(\d{3})(\d{3})(\d{1,3})/, '$1.$2.$3');
        } else if (value.length > 3) {
            value = value.replace(/(\d{3})(\d{1,3})/, '$1.$2');
        }
        
        input.value = value;
    }
    
    // Validar campo de CPF
    validateCPFField(field) {
        const cpf = field.value.replace(/\D/g, '');
        const isValid = this.isValidCPF(cpf);
        
        // Remover classes anteriores
        field.classList.remove('is-valid', 'is-invalid');
        
        // Remover mensagens anteriores
        const existingFeedback = field.parentNode.querySelector('.cpf-feedback');
        if (existingFeedback) {
            existingFeedback.remove();
        }
        
        if (cpf.length === 0) {
            // Campo vazio - não mostrar erro se não for obrigatório
            if (field.hasAttribute('required')) {
                this.showCPFError(field, 'CPF é obrigatório');
            }
            return false;
        }
        
        if (cpf.length < 11) {
            this.showCPFError(field, 'CPF deve conter 11 dígitos');
            return false;
        }
        
        if (!isValid) {
            this.showCPFError(field, 'CPF inválido');
            return false;
        }
        
        // CPF válido
        this.showCPFSuccess(field);
        return true;
    }
    
    // Validar CPF (algoritmo oficial)
    isValidCPF(cpf) {
        // Remover formatação
        cpf = cpf.replace(/\D/g, '');
        
        // Verificar se tem 11 dígitos
        if (cpf.length !== 11) return false;
        
        // Verificar se todos os dígitos são iguais
        if (/^(\d)\1{10}$/.test(cpf)) return false;
        
        // Validar primeiro dígito verificador
        let sum = 0;
        for (let i = 0; i < 9; i++) {
            sum += parseInt(cpf.charAt(i)) * (10 - i);
        }
        
        let remainder = 11 - (sum % 11);
        if (remainder === 10 || remainder === 11) remainder = 0;
        if (remainder !== parseInt(cpf.charAt(9))) return false;
        
        // Validar segundo dígito verificador
        sum = 0;
        for (let i = 0; i < 10; i++) {
            sum += parseInt(cpf.charAt(i)) * (11 - i);
        }
        
        remainder = 11 - (sum % 11);
        if (remainder === 10 || remainder === 11) remainder = 0;
        if (remainder !== parseInt(cpf.charAt(10))) return false;
        
        return true;
    }
    
    // Mostrar erro de CPF
    showCPFError(field, message) {
        field.classList.add('is-invalid');
        
        const feedback = document.createElement('div');
        feedback.className = 'invalid-feedback cpf-feedback';
        feedback.innerHTML = `<i class="fas fa-exclamation-triangle me-1"></i>${message}`;
        
        field.parentNode.appendChild(feedback);
    }
    
    // Mostrar sucesso de CPF
    showCPFSuccess(field) {
        field.classList.add('is-valid');
        
        const feedback = document.createElement('div');
        feedback.className = 'valid-feedback cpf-feedback';
        feedback.innerHTML = '<i class="fas fa-check-circle me-1"></i>CPF válido';
        
        field.parentNode.appendChild(feedback);
    }
    
    // Verificar duplicata de CPF (via AJAX)
    async checkCPFDuplicate(cpf, currentId = null) {
        try {
            const response = await fetch('/Admin/VerificarCPFDuplicado', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
                },
                body: JSON.stringify({ cpf, currentId })
            });
            
            const result = await response.json();
            return result.isDuplicate;
        } catch (error) {
            console.error('Erro ao verificar CPF duplicado:', error);
            return false;
        }
    }
    
    // Validar CPF com verificação de duplicata
    async validateCPFWithDuplicateCheck(field, currentId = null) {
        const basicValidation = this.validateCPFField(field);
        
        if (!basicValidation) return false;
        
        const cpf = field.value.replace(/\D/g, '');
        const isDuplicate = await this.checkCPFDuplicate(cpf, currentId);
        
        if (isDuplicate) {
            // Remover classe de sucesso
            field.classList.remove('is-valid');
            
            // Mostrar erro de duplicata
            const existingFeedback = field.parentNode.querySelector('.cpf-feedback');
            if (existingFeedback) {
                existingFeedback.remove();
            }
            
            this.showCPFError(field, 'Este CPF já está cadastrado no sistema');
            return false;
        }
        
        return true;
    }
    
    // Limpar CPF (remover formatação)
    static cleanCPF(cpf) {
        return cpf.replace(/\D/g, '');
    }
    
    // Formatar CPF para exibição
    static formatCPFDisplay(cpf) {
        cpf = cpf.replace(/\D/g, '');
        if (cpf.length === 11) {
            return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
        }
        return cpf;
    }
    
    // Mascarar CPF para privacidade (000.000.XXX-XX)
    static maskCPF(cpf) {
        cpf = cpf.replace(/\D/g, '');
        if (cpf.length === 11) {
            return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.XXX-XX');
        }
        return cpf;
    }
}

// Inicializar validador
const cpfValidator = new CPFValidator();

// Exportar para uso global
window.CPFValidator = CPFValidator;
window.cpfValidator = cpfValidator;

// Função utilitária global para validação rápida
window.isValidCPF = function(cpf) {
    return cpfValidator.isValidCPF(cpf);
};

// Função utilitária global para formatação
window.formatCPF = function(cpf) {
    return CPFValidator.formatCPFDisplay(cpf);
};

// Função utilitária global para limpeza
window.cleanCPF = function(cpf) {
    return CPFValidator.cleanCPF(cpf);
};

// CSS adicional para feedback visual
const cpfStyles = `
<style>
.is-valid {
    border-color: #28a745 !important;
    box-shadow: 0 0 0 0.2rem rgba(40, 167, 69, 0.25) !important;
}

.is-invalid {
    border-color: #dc3545 !important;
    box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.25) !important;
}

.valid-feedback {
    display: block;
    width: 100%;
    margin-top: 0.25rem;
    font-size: 0.875em;
    color: #28a745;
}

.invalid-feedback {
    display: block;
    width: 100%;
    margin-top: 0.25rem;
    font-size: 0.875em;
    color: #dc3545;
}

.cpf-feedback {
    font-weight: 500;
}

.cpf-feedback i {
    font-size: 0.9em;
}

/* Animação para feedback */
.cpf-feedback {
    animation: fadeInUp 0.3s ease-out;
}

@keyframes fadeInUp {
    from {
        opacity: 0;
        transform: translateY(10px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

/* Estilo para campos de CPF */
input[name*="CPF"], 
input[data-cpf], 
.cpf-input {
    font-family: 'Courier New', monospace;
    letter-spacing: 0.5px;
}

/* Placeholder personalizado */
input[name*="CPF"]::placeholder,
input[data-cpf]::placeholder,
.cpf-input::placeholder {
    font-family: inherit;
    letter-spacing: normal;
}
</style>
`;

// Adicionar estilos ao head
document.head.insertAdjacentHTML('beforeend', cpfStyles);

console.log('CPF Validator carregado - Sistema Petrobras');

