# Bookshop — Руководство по запуску
ОБНОВИТЕ ЗАВИСИМОСТИ С ПОМЕТКОЙ "УЯЗВИМО"

Порты можно изменить в docker-compose (при запуске docker) и прочих файлах конфигурации при локальном запуске

## Требования

- [Docker](https://www.docker.com/get-started) и Docker Compose
- Либо для локального запуска без Docker:
  - [.NET 10 SDK](https://dotnet.microsoft.com/download)
  - [PostgreSQL 15+](https://www.postgresql.org/download/)
  - [Node.js](https://nodejs.org/) (для фронтенда)

---

## Способ 1: Docker Compose (рекомендуется)

Запускает всё сразу: базу данных, бэкенд и фронтенд.

```bash
cd bookshop (или папка проекта в которой находятся файлы)
docker compose up --build
```

После запуска:

| Сервис    | Адрес                        |
|-----------|------------------------------|
| Фронтенд  | http://localhost:3000         |
| API       | http://localhost:5000         |
| Swagger   | http://localhost:8080/swagger |
| PostgreSQL| http://localhost:5435         |
| pgadmin4  | http://localhost:5050         |
| dufs      | http://localhost:5034         |
| Grafana   | http://localhost:3001         |

Остановить:

```bash
docker compose down
```

Остановить и удалить данные БД:

```bash
docker compose down -v
```

---

## Способ 2: Локальный запуск (без Docker)

### 1. База данных

Запустите PostgreSQL и создайте базу данных:

```sql
CREATE DATABASE bookshopdb;
```

Убедитесь, что пользователь `postgres` с паролем `postgres` имеет доступ, или обновите строку подключения в `bookshop/appsettings.Development.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "UserID=postgres;Host=localhost;port=5432;database=bookshopdb;Password=postgres"
}
```

### 2. Применить миграции
Миграции применяются автоматически, если этого не произошло, то выполните следущие действия, предварительно установив dotnet ef 
https://learn.microsoft.com/en-us/ef/core/cli/dotnet

Из корня репозитория (`bookshop/`):

```bash
dotnet ef database update --project Infrastructure --startup-project bookshop
```

### 3. Запустить бэкенд

```bash
cd bookshop/bookshop
dotnet run
```

API будет доступен по адресу (используется тот же адрес):
- http://localhost:5000
- Swagger UI: http://localhost:5000/swagger

### 4. Запустить фронтенд

```bash
cd bookshop/frontend
npm install
npm run dev
```

---

## Миграции

Добавить новую миграцию (из корня `bookshop/`):

```bash
dotnet ef migrations add <НазваниеМиграции> --project Infrastructure --startup-project bookshop
```

---

## Переменные окружения (Docker)

| Переменная                          | Значение по умолчанию                          |
|-------------------------------------|------------------------------------------------|
| `ConnectionStrings__DefaultConnection` | `UserID=postgres;Host=postgres;port=5432;database=bookshopdb;Password=postgres` |
| `Jwt__Key`                          | `bookshop-super-secret-jwt-key-32chars!!`      |
| `Jwt__Issuer`                       | `bookshop-api`                                 |
| `Jwt__Audience`                     | `bookshop-client`                              |
| `Jwt__ExpiresInMinutes`             | `1440`                                         |

> В продакшене обязательно замените `Jwt__Key` на собственный секретный ключ длиной не менее 32 символов.
