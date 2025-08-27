Sistema CRUD ASP.NET Core 8.0 com MySQL - Petrobras
Sistema web completo para gestão de moradores e administradores da Petrobras, desenvolvido em ASP.NET Core 8.0 com banco de dados MySQL.

🚀 Funcionalidades
👥 Para Moradores
Cadastro Completo: Dados pessoais e socioeconômicos
Visualização de Avisos: Comunicados da administração
Envio de Feedbacks: Sugestões e pedidos para a administração
Acompanhamento: Status dos feedbacks enviados
🛠️ Para Administradores
Dashboard Interativo: Estatísticas e gráficos em tempo real
Filtros Avançados: Por região, escolaridade, data
Gráficos Chart.js: Visualização de dados por região e escolaridade
Exportação: Relatórios em CSV e PDF
Moderação: Aprovação/rejeição de feedbacks
Gestão de Avisos: CRUD completo para comunicados
🔐 Autenticação e Autorização
ASP.NET Core Identity: Sistema robusto de autenticação
Roles: Admin e Morador com permissões específicas
Usuário Admin Padrão: admin@petrobras.com / Admin123!
🛠️ Tecnologias Utilizadas
Backend: ASP.NET Core 8.0 MVC
Banco de Dados: MySQL
ORM: Entity Framework Core
Autenticação: ASP.NET Core Identity
Frontend: Bootstrap 5 + Razor Views
Gráficos: Chart.js
PDF: QuestPDF
CSV: CsvHelper
Icons: Font Awesome
📁 Estrutura do Projeto
SistemaPetrobras/
├── Controllers/           # Controllers MVC
├── Models/               # Modelos de dados
├── Views/                # Views Razor
├── Data/                 # DbContext
├── Program.cs            # Configuração da aplicação
├── appsettings.json      # Configurações
└── SistemaPetrobras.csproj
🚀 Como Executar
Pré-requisitos:

.NET 8.0 SDK
MySQL Server
Configuração:

Configure a string de conexão no appsettings.json
Execute dotnet restore
Execute dotnet ef database update
Execução:

dotnet run
Acesso:

URL: https://localhost:5001
Admin: admin@petrobras.com / Admin123!
📊 Recursos Implementados
✅ CRUD completo para moradores
✅ Sistema de avisos
✅ Feedback com moderação
✅ Dashboard com gráficos interativos
✅ Filtros dinâmicos
✅ Exportação CSV/PDF
✅ Autenticação por roles
✅ Interface responsiva
✅ Validações de formulário
📖 Documentação
Consulte o arquivo INSTALACAO.md para instruções detalhadas de instalação e configuração.

🎯 Características Técnicas
Arquitetura: MVC Pattern
Responsivo: Bootstrap 5
Segurança: Identity + Authorization
Performance: Entity Framework Core
Usabilidade: Interface intuitiva
Relatórios: Exportação automática
Desenvolvido para atender aos requisitos da Petrobras com foco em simplicidade, funcionalidade e manutenibilidade.
