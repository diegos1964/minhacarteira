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
- Scalar (Documentação da API)

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
- Scalar: https://localhost:7266/scalar

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
  - Body: `{ "name": "string", "email": "string", "password": "string","confirmPassword": "string", "cpf": "string" }`
- `POST /api/auth/login` - Login de usuário
  - Body: `{ "email": "string", "password": "string" }`
- `GET /api/auth/me` - Obtém informações do usuário logado
  - Requer autenticação

### Carteiras
- `GET /api/wallet` - Lista todas as carteiras do usuário
  - Query params: `page`, `pageSize`, `search`
  - Requer autenticação
- `GET /api/wallet/{id}` - Obtém detalhes de uma carteira
  - Requer autenticação
- `POST /api/wallet` - Cria uma nova carteira
  - Body: `{ "name": "string", "description": "string" }`
  - Requer autenticação
- `PUT /api/wallet/{id}` - Atualiza uma carteira
  - Body: `{ "name": "string", "description": "string" }`
  - Requer autenticação
- `DELETE /api/wallet/{id}` - Remove uma carteira
  - Requer autenticação
- `GET /api/wallet/balance` - Obtém o saldo total do usuário
  - Requer autenticação
- `GET /api/wallet/transfer-info/{walletId}` - Obtém informações para transferência
  - Retorna: `{ "id": number, "name": "string", "ownerName": "string", "ownerCpf": "string" }`
  - Requer autenticação

### Transações
- `GET /api/transaction` - Lista todas as transações
  - Query params: `page`, `pageSize`, `startDate`, `endDate`, `type`, `walletId`
  - Requer autenticação
- `GET /api/transaction/{id}` - Obtém detalhes de uma transação
  - Requer autenticação
- `POST /api/transaction` - Cria uma nova transação
  - Body: `{ "type": "Income|Expense", "amount": number, "description": "string", "walletId": number, "date": "date" }`
  - Requer autenticação
- `PATCH /api/transaction/{id}` - Atualiza uma transação
  - Body: `{ "type": "Income|Expense", "amount": number, "description": "string", "date": "date" }`
  - Requer autenticação
- `DELETE /api/transaction/{id}` - Remove uma transação
  - Requer autenticação
- `POST /api/transaction/transfer` - Realiza uma transferência entre carteiras
  - Body: `{ "sourceWalletId": number, "targetWalletId": number, "amount": number, "description": "string" }`
  - Requer autenticação
- `GET /api/transaction/wallet/{walletId}/income` - Obtém receita total da carteira
  - Requer autenticação
- `GET /api/transaction/wallet/{walletId}/expense` - Obtém despesa total da carteira
  - Requer autenticação

## Documentação da API

A API possui três interfaces de documentação:

1. **Swagger UI**: Interface interativa para testar os endpoints
   - URL: https://localhost:7266/swagger

2. **ReDoc**: Documentação em formato de página única
   - URL: https://localhost:7266/redoc

3. **Scalar**: Interface moderna e interativa
   - URL: https://localhost:7266/scalar

## Contribuição

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/nova-feature`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova feature'`)
4. Push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request 