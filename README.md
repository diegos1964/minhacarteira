# Minha Carteira API

API para gerenciamento de carteiras e transações financeiras.

## Funcionalidades

- Autenticação de usuários
- Gerenciamento de carteiras
- Registro de transações (entradas e saídas)
- Transferências entre carteiras
- Filtros de transações por período
- Consulta de saldo

## Tecnologias Utilizadas

- .NET 9.0
- Entity Framework Core
- PostgreSQL
- JWT Authentication
- Swagger/OpenAPI

## Pré-requisitos

- .NET 9.0 SDK
- Docker (para o PostgreSQL)
- Git

## Configuração do Ambiente

1. Clone o repositório:
```bash
git clone https://github.com/seu-usuario/minhacarteira.git
cd minhacarteira
```

2. Inicie o container do PostgreSQL:
```bash
docker run --name postgres-minhacarteira -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres
```

3. Restaure as dependências e execute as migrações:
```bash
dotnet restore
dotnet ef database update --project MinhaCarteira.API
```

4. Execute a API:
```bash
cd MinhaCarteira.API
dotnet run
```

A API estará disponível em `https://localhost:7001` e o Swagger em `https://localhost:7001/swagger`.

## Usuários de Teste

O sistema vem com três usuários pré-cadastrados:

1. João Silva
   - Email: joao@email.com
   - Senha: 123456

2. Maria Santos
   - Email: maria@email.com
   - Senha: 123456

3. Pedro Oliveira
   - Email: pedro@email.com
   - Senha: 123456

Cada usuário possui duas carteiras:
- Carteira Principal
- Carteira Poupança

## Endpoints Principais

### Autenticação
- POST /api/auth/register - Registro de novo usuário
- POST /api/auth/login - Login de usuário

### Carteiras
- GET /api/wallet - Lista todas as carteiras do usuário
- POST /api/wallet - Cria uma nova carteira
- GET /api/wallet/{id} - Obtém detalhes de uma carteira
- PUT /api/wallet/{id} - Atualiza uma carteira
- DELETE /api/wallet/{id} - Remove uma carteira
- GET /api/wallet/balance - Obtém o saldo total do usuário

### Transações
- GET /api/transaction - Lista todas as transações (com filtros opcionais)
- POST /api/transaction - Cria uma nova transação
- POST /api/transaction/transfer - Realiza uma transferência entre carteiras
- GET /api/transaction/{id} - Obtém detalhes de uma transação
- PUT /api/transaction/{id} - Atualiza uma transação
- DELETE /api/transaction/{id} - Remove uma transação

## Contribuição

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/nova-feature`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova feature'`)
4. Push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request 