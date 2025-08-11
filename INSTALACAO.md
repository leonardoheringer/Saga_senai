# Sistema CRUD ASP.NET Core 8.0 com MySQL - Petrobras

## Pré-requisitos

1. **.NET 8.0 SDK** - [Download aqui](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **MySQL Server** - [Download aqui](https://dev.mysql.com/downloads/mysql/)
3. **Visual Studio 2022** ou **Visual Studio Code** (opcional)

## Passo a Passo para Instalação

### 1. Configurar o Banco de Dados MySQL

1. Instale o MySQL Server
2. Crie um banco de dados:
```sql
CREATE DATABASE SistemaPetrobras;
```

3. Crie um usuário (opcional):
```sql
CREATE USER 'petrobras'@'localhost' IDENTIFIED BY 'sua_senha_aqui';
GRANT ALL PRIVILEGES ON SistemaPetrobras.* TO 'petrobras'@'localhost';
FLUSH PRIVILEGES;
```

### 2. Configurar a String de Conexão

Edite o arquivo `appsettings.json` e configure a string de conexão:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SistemaPetrobras;User=root;Password=sua_senha_aqui;"
  }
}
```

**Substitua `sua_senha_aqui` pela senha do seu MySQL.**

### 3. Instalar Dependências

Abra o terminal na pasta do projeto e execute:

```bash
dotnet restore
```

### 4. Executar Migrations

Execute os comandos para criar as tabelas no banco:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**Nota:** Se o comando `dotnet ef` não for reconhecido, instale a ferramenta:
```bash
dotnet tool install --global dotnet-ef
```

### 5. Executar o Sistema

Execute o comando para iniciar a aplicação:

```bash
dotnet run
```

O sistema estará disponível em: `https://localhost:5001` ou `http://localhost:5000`

## Usuários Padrão

### Administrador
- **Email:** admin@petrobras.com
- **Senha:** Admin123!

### Para Moradores
- Registre-se como novo usuário
- Será automaticamente atribuído o papel de "Morador"

## Funcionalidades Implementadas

### Para Moradores:
- ✅ Cadastro de dados pessoais e socioeconômicos
- ✅ Visualização de avisos da administração
- ✅ Envio de sugestões/pedidos (feedback)
- ✅ Acompanhamento do status dos feedbacks

### Para Administradores:
- ✅ Dashboard com estatísticas e gráficos
- ✅ Filtros por região, escolaridade e data
- ✅ Gráficos interativos (Chart.js)
- ✅ Exportação de relatórios em CSV e PDF
- ✅ Moderação de feedbacks (aprovar/rejeitar)
- ✅ Gerenciamento de avisos (CRUD)

### Tecnologias Utilizadas:
- ✅ ASP.NET Core 8.0 MVC
- ✅ Entity Framework Core
- ✅ MySQL (Pomelo.EntityFrameworkCore.MySql)
- ✅ ASP.NET Core Identity
- ✅ Bootstrap 5
- ✅ Chart.js
- ✅ QuestPDF (geração de PDF)
- ✅ CsvHelper (exportação CSV)

## Estrutura do Projeto

```
SistemaPetrobras/
├── Controllers/
│   ├── HomeController.cs
│   ├── MoradorController.cs
│   └── AdminController.cs
├── Models/
│   ├── Morador.cs
│   ├── Aviso.cs
│   └── Feedback.cs
├── Views/
│   ├── Home/
│   ├── Morador/
│   ├── Admin/
│   └── Shared/
├── Data/
│   └── ApplicationDbContext.cs
├── Program.cs
├── appsettings.json
└── SistemaPetrobras.csproj
```

## Solução de Problemas

### Erro de Conexão com MySQL
- Verifique se o MySQL está rodando
- Confirme a string de conexão no `appsettings.json`
- Teste a conexão com um cliente MySQL

### Erro ao executar migrations
- Certifique-se de que o `dotnet-ef` está instalado
- Verifique se o banco de dados existe
- Confirme as permissões do usuário MySQL

### Erro 404 ao acessar páginas
- Verifique se o projeto está rodando
- Confirme a URL no navegador
- Verifique se o usuário tem as permissões corretas (Admin/Morador)

## Próximos Passos (Melhorias Futuras)

1. Implementar paginação nas listagens
2. Adicionar validação de CPF
3. Implementar upload de arquivos nos feedbacks
4. Adicionar notificações por email
5. Implementar cache para melhor performance
6. Adicionar logs de auditoria
7. Implementar testes unitários

## Suporte

Para dúvidas ou problemas, consulte a documentação oficial:
- [ASP.NET Core](https://docs.microsoft.com/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [MySQL](https://dev.mysql.com/doc/)

