
# Fullstack App — .NET 9 + Angular + Postgres (Docker)

Projeto fullstack com:

- ✅ **.NET 9 API**
- ✅ **Angular Front**
- ✅ **PostgreSQL**
- ✅ **Docker + Docker Compose**
- ✅ Autenticação com JWT
- ✅ Base para CQRS, Clean Architecture, Domain Driven

---

## Estrutura do Projeto

```
/back
  /src
    /FullstackApp.Api
    /FullstackApp.Application
    /FullstackApp.Infrastructure
    /FullstackApp.Domain
  Dockerfile

/front
  /fullstack-web
  Dockerfile

docker-compose.yml
.env
```

---

## Variáveis de ambiente

Crie um arquivo `.env` na raiz:

```
# DATABASE
DATABASE_CONNECTION=Host=postgres;Port=5432;Database=fullstackdb;Username=dev;Password=dev123

# JWT
JWT_KEY=uma chave forte aqui
JWT_ISSUER=fullstackapp
JWT_AUDIENCE=fullstackapp_users
JWT_EXPIRE_MINUTES=60

# Swagger login (se configurado)
SWAGGERSECURITY_USER=admin
SWAGGERSECURITY_PASSWORD=admin
```

---

## Subindo tudo com Docker

### Build

```bash
docker compose build
```

### Subir containers

```bash
docker compose up -d
```

### Ver logs (opcional)

```bash
docker compose logs -f
```

---

## Acessos

| Serviço | URL |
|--------|-----|
Frontend Angular | http://localhost:4200  
API .NET | http://localhost:8080  
Swagger | http://localhost:8080/swagger  

---

## Testes Manuais

### Registrar usuário
POST `http://localhost:8080/auth/register`

Body:

```json
{
  "name": "Fabio",
  "email": "fabio@test.com",
  "password": "123456"
}
```

### Login
POST `http://localhost:8080/auth/login`

Body:

```json
{
  "email": "fabio@test.com",
  "password": "123456"
}
```

### Buscar usuários
GET `http://localhost:8080/users`

Header:

```
Authorization: Bearer TOKEN
```

---

## Parar containers

```bash
docker compose down
```

---

## Tecnologias e padrões

- MediatR
- Soft delete (DeletedAt)
- BCrypt hashing
- JWT Bearer Auth
- Interceptor Angular para token
- Angular Guard para rotas privadas (`/users`)

---

Feito por **Fabio Rattis**
