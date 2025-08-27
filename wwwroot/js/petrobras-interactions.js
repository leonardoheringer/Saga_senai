// Interações Avançadas - Sistema Petrobras
// Micro-interações, animações e funcionalidades modernas

document.addEventListener('DOMContentLoaded', function() {
    
    // Inicializar todas as funcionalidades
    initializeAnimations();
    initializeFormValidations();
    initializeLoadingStates();
    initializeTooltips();
    initializeSearchFilters();
    
    console.log('Sistema Petrobras - Interações carregadas');
});

// Animações e Micro-interações
function initializeAnimations() {
    // Animação de entrada para elementos
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };
    
    const observer = new IntersectionObserver(function(entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate-in');
            }
        });
    }, observerOptions);
    
    // Observar elementos com animação
    document.querySelectorAll('.fade-in-up, .fade-in').forEach(el => {
        observer.observe(el);
    });
    
    // Efeito de hover nos cards
    document.querySelectorAll('.card').forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-2px)';
            this.style.boxShadow = '0 10px 25px rgba(0,0,0,0.15)';
        });
        
        card.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0)';
            this.style.boxShadow = '';
        });
    });
    
    // Efeito ripple nos botões
    document.querySelectorAll('.btn').forEach(button => {
        button.addEventListener('click', createRipple);
    });
}

// Efeito Ripple
function createRipple(event) {
    const button = event.currentTarget;
    const circle = document.createElement('span');
    const diameter = Math.max(button.clientWidth, button.clientHeight);
    const radius = diameter / 2;
    
    const rect = button.getBoundingClientRect();
    circle.style.width = circle.style.height = `${diameter}px`;
    circle.style.left = `${event.clientX - rect.left - radius}px`;
    circle.style.top = `${event.clientY - rect.top - radius}px`;
    circle.classList.add('ripple');
    
    const ripple = button.getElementsByClassName('ripple')[0];
    if (ripple) {
        ripple.remove();
    }
    
    button.appendChild(circle);
    
    // Remover o elemento após a animação
    setTimeout(() => {
        circle.remove();
    }, 600);
}

// Validações de Formulário Avançadas
function initializeFormValidations() {
    const forms = document.querySelectorAll('form');
    
    forms.forEach(form => {
        const inputs = form.querySelectorAll('input, select, textarea');
        
        inputs.forEach(input => {
            // Validação em tempo real
            input.addEventListener('blur', function() {
                validateField(this);
            });
            
            input.addEventListener('input', function() {
                clearFieldError(this);
            });
        });
        
        // Validação no submit
        form.addEventListener('submit', function(e) {
            let isValid = true;
            
            inputs.forEach(input => {
                if (!validateField(input)) {
                    isValid = false;
                }
            });
            
            if (!isValid) {
                e.preventDefault();
                showFormError('Por favor, corrija os erros antes de continuar.');
            }
        });
    });
}

// Validar campo individual
function validateField(field) {
    const value = field.value.trim();
    const fieldType = field.type;
    const isRequired = field.hasAttribute('required');
    
    // Limpar erros anteriores
    clearFieldError(field);
    
    // Validação de campo obrigatório
    if (isRequired && !value) {
        showFieldError(field, 'Este campo é obrigatório.');
        return false;
    }
    
    // Validações específicas por tipo
    switch (fieldType) {
        case 'email':
            if (value && !isValidEmail(value)) {
                showFieldError(field, 'Por favor, insira um email válido.');
                return false;
            }
            break;
            
        case 'tel':
            if (value && !isValidPhone(value)) {
                showFieldError(field, 'Por favor, insira um telefone válido.');
                return false;
            }
            break;
    }
    
    // Validação de CPF
    if (field.name === 'CPF' && value && !isValidCPF(value)) {
        showFieldError(field, 'Por favor, insira um CPF válido.');
        return false;
    }
    
    return true;
}

// Mostrar erro no campo
function showFieldError(field, message) {
    field.classList.add('is-invalid');
    
    let errorDiv = field.parentNode.querySelector('.invalid-feedback');
    if (!errorDiv) {
        errorDiv = document.createElement('div');
        errorDiv.className = 'invalid-feedback';
        field.parentNode.appendChild(errorDiv);
    }
    
    errorDiv.textContent = message;
}

// Limpar erro do campo
function clearFieldError(field) {
    field.classList.remove('is-invalid');
    const errorDiv = field.parentNode.querySelector('.invalid-feedback');
    if (errorDiv) {
        errorDiv.remove();
    }
}

// Mostrar erro geral do formulário
function showFormError(message) {
    // Remover alertas anteriores
    const existingAlert = document.querySelector('.form-error-alert');
    if (existingAlert) {
        existingAlert.remove();
    }
    
    const alert = document.createElement('div');
    alert.className = 'alert alert-danger form-error-alert fade-in';
    alert.innerHTML = `
        <i class="fas fa-exclamation-triangle me-2"></i>
        ${message}
        <button type="button" class="btn-close" onclick="this.parentNode.remove()"></button>
    `;
    
    const form = document.querySelector('form');
    form.insertBefore(alert, form.firstChild);
    
    // Scroll para o topo do formulário
    form.scrollIntoView({ behavior: 'smooth', block: 'start' });
}

// Estados de Loading
function initializeLoadingStates() {
    // Loading nos botões de submit
    document.querySelectorAll('form').forEach(form => {
        form.addEventListener('submit', function() {
            const submitBtn = form.querySelector('button[type="submit"], input[type="submit"]');
            if (submitBtn) {
                showButtonLoading(submitBtn);
            }
        });
    });
    
    // Loading nos links de exportação
    document.querySelectorAll('a[href*="Exportar"]').forEach(link => {
        link.addEventListener('click', function() {
            showButtonLoading(this);
            
            // Remover loading após 3 segundos
            setTimeout(() => {
                hideButtonLoading(this);
            }, 3000);
        });
    });
}

// Mostrar loading no botão
function showButtonLoading(button) {
    button.classList.add('loading');
    button.disabled = true;
    
    const originalText = button.textContent;
    button.setAttribute('data-original-text', originalText);
    
    if (button.tagName === 'BUTTON') {
        button.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Carregando...';
    }
}

// Esconder loading do botão
function hideButtonLoading(button) {
    button.classList.remove('loading');
    button.disabled = false;
    
    const originalText = button.getAttribute('data-original-text');
    if (originalText) {
        button.textContent = originalText;
    }
}

// Tooltips
function initializeTooltips() {
    // Inicializar tooltips do Bootstrap se disponível
    if (typeof bootstrap !== 'undefined') {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function(tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }
}

// Filtros de Busca Avançada
function initializeSearchFilters() {
    const searchInputs = document.querySelectorAll('input[type="search"], .search-input');
    
    searchInputs.forEach(input => {
        let searchTimeout;
        
        input.addEventListener('input', function() {
            clearTimeout(searchTimeout);
            
            searchTimeout = setTimeout(() => {
                performSearch(this.value, this);
            }, 300);
        });
    });
}

// Realizar busca
function performSearch(query, input) {
    const targetTable = input.getAttribute('data-target') || 'table tbody';
    const rows = document.querySelectorAll(targetTable + ' tr');
    
    rows.forEach(row => {
        const text = row.textContent.toLowerCase();
        const matches = text.includes(query.toLowerCase());
        
        row.style.display = matches ? '' : 'none';
        
        if (matches && query) {
            row.classList.add('search-highlight');
        } else {
            row.classList.remove('search-highlight');
        }
    });
    
    // Mostrar mensagem se nenhum resultado
    updateSearchResults(rows, query);
}

// Atualizar resultados da busca
function updateSearchResults(rows, query) {
    const visibleRows = Array.from(rows).filter(row => row.style.display !== 'none');
    const table = rows[0]?.closest('table');
    
    if (!table) return;
    
    // Remover mensagem anterior
    const existingMessage = table.parentNode.querySelector('.search-no-results');
    if (existingMessage) {
        existingMessage.remove();
    }
    
    // Mostrar mensagem se não há resultados
    if (visibleRows.length === 0 && query) {
        const message = document.createElement('div');
        message.className = 'search-no-results alert alert-info mt-3';
        message.innerHTML = `
            <i class="fas fa-search me-2"></i>
            Nenhum resultado encontrado para "${query}".
        `;
        table.parentNode.appendChild(message);
    }
}

// Utilitários de Validação
function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

function isValidPhone(phone) {
    const phoneRegex = /^[\d\s\-\(\)\+]{10,}$/;
    return phoneRegex.test(phone);
}

function isValidCPF(cpf) {
    cpf = cpf.replace(/[^\d]/g, '');
    
    if (cpf.length !== 11) return false;
    if (/^(\d)\1{10}$/.test(cpf)) return false;
    
    let sum = 0;
    for (let i = 0; i < 9; i++) {
        sum += parseInt(cpf.charAt(i)) * (10 - i);
    }
    
    let remainder = 11 - (sum % 11);
    if (remainder === 10 || remainder === 11) remainder = 0;
    if (remainder !== parseInt(cpf.charAt(9))) return false;
    
    sum = 0;
    for (let i = 0; i < 10; i++) {
        sum += parseInt(cpf.charAt(i)) * (11 - i);
    }
    
    remainder = 11 - (sum % 11);
    if (remainder === 10 || remainder === 11) remainder = 0;
    if (remainder !== parseInt(cpf.charAt(10))) return false;
    
    return true;
}

// Formatação de campos
function formatCPF(input) {
    let value = input.value.replace(/\D/g, '');
    value = value.replace(/(\d{3})(\d)/, '$1.$2');
    value = value.replace(/(\d{3})(\d)/, '$1.$2');
    value = value.replace(/(\d{3})(\d{1,2})$/, '$1-$2');
    input.value = value;
}

function formatPhone(input) {
    let value = input.value.replace(/\D/g, '');
    value = value.replace(/(\d{2})(\d)/, '($1) $2');
    value = value.replace(/(\d{4})(\d)/, '$1-$2');
    value = value.replace(/(\d{4})-(\d)(\d{4})/, '$1$2-$3');
    input.value = value;
}

// Aplicar formatação automática
document.addEventListener('DOMContentLoaded', function() {
    // CPF
    document.querySelectorAll('input[name="CPF"], .cpf-input').forEach(input => {
        input.addEventListener('input', function() {
            formatCPF(this);
        });
    });
    
    // Telefone
    document.querySelectorAll('input[type="tel"], .phone-input').forEach(input => {
        input.addEventListener('input', function() {
            formatPhone(this);
        });
    });
});

// Notificações Toast
function showToast(message, type = 'success') {
    const toastContainer = getOrCreateToastContainer();
    
    const toast = document.createElement('div');
    toast.className = `toast align-items-center text-white bg-${type} border-0 fade-in`;
    toast.setAttribute('role', 'alert');
    toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">
                <i class="fas fa-${getToastIcon(type)} me-2"></i>
                ${message}
            </div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" onclick="this.parentNode.parentNode.remove()"></button>
        </div>
    `;
    
    toastContainer.appendChild(toast);
    
    // Auto-remover após 5 segundos
    setTimeout(() => {
        toast.remove();
    }, 5000);
}

function getOrCreateToastContainer() {
    let container = document.querySelector('.toast-container');
    if (!container) {
        container = document.createElement('div');
        container.className = 'toast-container position-fixed top-0 end-0 p-3';
        container.style.zIndex = '9999';
        document.body.appendChild(container);
    }
    return container;
}

function getToastIcon(type) {
    const icons = {
        success: 'check-circle',
        danger: 'exclamation-triangle',
        warning: 'exclamation-circle',
        info: 'info-circle'
    };
    return icons[type] || 'info-circle';
}

// CSS adicional para as interações
const additionalStyles = `
<style>
.ripple {
    position: absolute;
    border-radius: 50%;
    transform: scale(0);
    animation: ripple 600ms linear;
    background-color: rgba(255, 255, 255, 0.6);
    pointer-events: none;
}

@keyframes ripple {
    to {
        transform: scale(4);
        opacity: 0;
    }
}

.animate-in {
    animation: fadeInUp 0.6s ease-out forwards;
}

.search-highlight {
    background-color: rgba(0, 168, 89, 0.1) !important;
}

.search-no-results {
    text-align: center;
    margin: 1rem 0;
}

.is-invalid {
    border-color: #dc3545 !important;
    box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.25) !important;
}

.invalid-feedback {
    display: block;
    width: 100%;
    margin-top: 0.25rem;
    font-size: 0.875em;
    color: #dc3545;
}

.form-error-alert {
    position: relative;
    margin-bottom: 1rem;
}

.btn.loading {
    pointer-events: none;
    opacity: 0.7;
}

.toast-container .toast {
    margin-bottom: 0.5rem;
}
</style>
`;

// Adicionar estilos ao head
document.head.insertAdjacentHTML('beforeend', additionalStyles);

