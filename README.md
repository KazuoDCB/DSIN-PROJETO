# DSIN-PROJETO - Cabeleleira Leila

Sistema web para gestao de um salao de beleza, com area publica para clientes, autenticacao separada por perfil, agendamento de servicos e painel administrativo para acompanhar clientes, servicos, agenda, dashboard e relatorios.

## Stack

### Backend
- C# / ASP.NET Core `net10.0`
- Entity Framework Core 10
- PostgreSQL via `Npgsql.EntityFrameworkCore.PostgreSQL`
- JWT Bearer Authentication
- Argon2Id para hash de senha
- Mapster para mapeamento entre entidades e DTOs
- OpenAPI nativo do ASP.NET Core

### Frontend
- React 19
- TypeScript 6
- Vite 8
- React Router DOM
- CSS modularizado por responsabilidade em `src/styles`

## Requisitos

- .NET SDK 10
- Node.js com npm
- PostgreSQL local

## Configuracao

O backend usa a connection string em:

```text
Back/cabeleleira-leila/cabeleleira-leila/appsettings.Development.json
```

Padrao atual:

```json
"DefaultConnection": "Server=localhost;Port=5432;Database=cabeleleira-leila;User Id=postgres;Password=13111726;"
```

Ajuste usuario/senha/porta conforme o PostgreSQL local.

O admin inicial e criado no start da API quando nao existe usuario com o email configurado:

```json
"AdminCredentials": {
  "Name": "Administrativo Leila",
  "Email": "admin@leila.com",
  "Password": "Admin@123"
}
```

## Rodando o backend

```powershell
cd Back/cabeleleira-leila/cabeleleira-leila
dotnet restore
dotnet ef database update
dotnet run
```

API em desenvolvimento:

```text
http://localhost:5165
https://localhost:7253
```

## Rodando o frontend

```powershell
cd Front/Cabeleleira_Leila
npm install
npm run dev
```

Frontend:

```text
http://localhost:5173
```

Por padrao o front consome:

```text
http://localhost:5165
```

Para sobrescrever, defina `VITE_API_BASE_URL` no ambiente do Vite.

## Builds

Backend:

```powershell
dotnet build Back/cabeleleira-leila/cabeleleira-leila/cabeleleira-leila.csproj
```

Frontend:

```powershell
cd Front/Cabeleleira_Leila
npm run build
```

## Resumo funcional

- Cadastro e login de clientes.
- Login administrativo separado por role.
- Clientes podem visualizar servicos e gerenciar seus agendamentos.
- Administradores podem gerenciar clientes, servicos, agenda e status dos atendimentos.
- Servicos possuem nome, descricao, preco, duracao e status.
- Agendamentos validam cliente, servicos ativos, data, horario ocupado e regras de alteracao.
- O backend centraliza as validacoes de negocio e retorna erros estruturados em `ErrorMessage`.
- O frontend exibe os erros retornados pela API sem aplicar validacao HTML nos formularios.
