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
- ReDoc
- Docker
- CORS (Cross-Origin Resource Sharing)

## Pré-requisitos

- .NET 9.0 SDK
- Docker e Docker Compose
- Git

## Configuração do Ambiente

1. Clone o repositório:
```bash
git clone https://github.com/seu-usuario/minhacarteira.git
cd minhacarteira
```

2. Gere o certificado de desenvolvimento HTTPS:
```bash
dotnet dev-certs https --export-path ./aspnetapp.pfx --password "senha123!"
```

3. Inicie os containers com Docker Compose:
```bash
docker-compose up --build
```

A API estará disponível em:
- HTTP: http://localhost:5037
- HTTPS: https://localhost:7266
- Swagger UI: https://localhost:7266/swagger
- ReDoc: https://localhost:7266/redoc

## Configuração do CORS

A API está configurada para aceitar requisições dos seguintes origens em desenvolvimento:
- http://localhost:3000 (React)
- http://localhost:5173 (Vite)
- http://localhost:4200 (Angular)
- http://localhost:8080 (Vue)

Para adicionar novas origens em produção, atualize a configuração CORS no arquivo `Program.cs`.

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

## Endpoints da API

### Autenticação
- `POST /api/auth/register` - Registro de novo usuário
  - Body: `{ "name": "string", "email": "string", "password": "string", "confirmPassword": "string", "cpf": "string" }`
  - Resposta: `{ "success": true, "data": { "token": "string", "user": { "id": number, "name": "string", "email": "string", "cpf": "string" } } }`
- `POST /api/auth/login` - Login de usuário
  - Body: `{ "email": "string", "password": "string" }`
  - Resposta: `{ "success": true, "data": { "token": "string", "user": { "id": number, "name": "string", "email": "string", "cpf": "string" } } }`
- `GET /api/auth/me` - Obtém informações do usuário logado
  - Requer autenticação
  - Resposta: `{ "success": true, "data": { "id": number, "name": "string", "email": "string", "cpf": "string" } }`

### Carteiras
- `GET /api/wallet` - Lista todas as carteiras do usuário
  - Query params: `page`, `pageSize`, `search`
  - Requer autenticação
  - Resposta: `{ "success": true, "data": { "items": [], "totalItems": number, "pageNumber": number, "pageSize": number, "totalPages": number } }`
- `GET /api/wallet/{id}` - Obtém detalhes de uma carteira
  - Requer autenticação
  - Resposta: `{ "success": true, "data": { "id": number, "name": "string", "balance": number, "createdAt": "date", "updatedAt": "date" } }`
- `POST /api/wallet` - Cria uma nova carteira
  - Body: `{ "name": "string", "initialBalance": number }`
  - Requer autenticação
  - Resposta: `{ "success": true, "data": { "id": number, "name": "string", "balance": number, "createdAt": "date", "updatedAt": "date" } }`
- `PATCH /api/wallet/{id}` - Atualiza uma carteira
  - Body: `{ "name": "string" }`
  - Requer autenticação
  - Resposta: `{ "success": true, "message": "Carteira atualizada com sucesso" }`
- `DELETE /api/wallet/{id}` - Remove uma carteira
  - Requer autenticação
  - Resposta: `{ "success": true, "message": "Carteira excluída com sucesso" }`
- `GET /api/wallet/balance` - Obtém o saldo total do usuário
  - Requer autenticação
  - Resposta: `{ "success": true, "data": { "totalBalance": number, "wallets": [{ "id": number, "name": "string", "balance": number }] } }`
- `GET /api/wallet/transfer-info/{walletId}` - Obtém informações para transferência
  - Requer autenticação
  - Resposta: `{ "success": true, "data": { "walletId": number, "walletName": "string", "ownerName": "string", "ownerEmail": "string", "ownerCPF": "string", "createdAt": "date" } }`
- `GET /api/wallet/transfer-info/email/{email}` - Obtém informações para transferência por email
  - Requer autenticação
  - Resposta: `{ "success": true, "data": [{ "walletId": number, "walletName": "string", "ownerName": "string", "ownerEmail": "string", "ownerCPF": "string", "createdAt": "date" }] }`
- `GET /api/wallet/transfer-info/cpf/{cpf}` - Obtém informações para transferência por CPF
  - Requer autenticação
  - Resposta: `{ "success": true, "data": [{ "walletId": number, "walletName": "string", "ownerName": "string", "ownerEmail": "string", "ownerCPF": "string", "createdAt": "date" }] }`

### Transações
- `GET /api/transaction/types` - Lista os tipos de transação disponíveis
  - Requer autenticação
  - Resposta: `{ "success": true, "data": { "types": ["Income", "Expense", "Transfer"] } }`
- `GET /api/transaction` - Lista todas as transações
  - Query params: `page`, `pageSize`, `startDate`, `endDate`, `type`, `walletId`
  - Requer autenticação
  - Resposta: `{ "success": true, "data": { "items": [], "totalItems": number, "pageNumber": number, "pageSize": number, "totalPages": number } }`
- `GET /api/transaction/{id}` - Obtém detalhes de uma transação
  - Requer autenticação
  - Resposta: `{ "success": true, "data": { "id": number, "type": "Income|Expense", "amount": number, "description": "string", "walletId": number, "date": "date" } }`
- `POST /api/transaction` - Cria uma nova transação
  - Body: `{ "type": "Income|Expense", "amount": number, "description": "string", "walletId": number, "date": "date" }`
  - Requer autenticação
  - Resposta: `{ "success": true, "data": { "id": number, "type": "Income|Expense", "amount": number, "description": "string", "walletId": number, "date": "date" } }`
- `PATCH /api/transaction/{id}` - Atualiza uma transação
  - Body: `{ "type": "Income|Expense", "amount": number, "description": "string", "date": "date" }`
  - Requer autenticação
  - Resposta: `{ "success": true, "message": "Transação atualizada com sucesso" }`
- `DELETE /api/transaction/{id}` - Remove uma transação
  - Requer autenticação
  - Resposta: `{ "success": true, "message": "Transação excluída com sucesso" }`
- `POST /api/transaction/transfer` - Realiza uma transferência entre carteiras
  - Body: `{ "sourceWalletId": number, "targetWalletId": number, "amount": number, "description": "string" }`
  - Requer autenticação
  - Resposta: `{ "success": true, "data": { "id": number, "type": "Transfer", "amount": number, "description": "string", "walletId": number, "date": "date" } }`
- `GET /api/transaction/wallet/{walletId}/income` - Obtém receita total da carteira
  - Requer autenticação
  - Resposta: `{ "success": true, "data": { "totalIncome": number, "transactions": [] } }`
- `GET /api/transaction/wallet/{walletId}/expense` - Obtém despesa total da carteira
  - Requer autenticação
  - Resposta: `{ "success": true, "data": { "totalExpense": number, "transactions": [] } }`

## Documentação da API

A API possui duas interfaces de documentação:

1. **Swagger UI**: Interface interativa para testar os endpoints
   - URL: https://localhost:7266/swagger

2. **ReDoc**: Documentação em formato de página única
   - URL: https://localhost:7266/redoc

## Contribuição

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/nova-feature`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova feature'`)
4. Push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request 