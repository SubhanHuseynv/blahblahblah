# Todo App

Sadə Todo tətbiqi — **React (Vite)** frontend + **ASP.NET Core 8 Web API** backend.

## Struktur

```
TodoApp/
├── backend/TodoApp.API/
│   ├── Controllers/     TodosController (REST endpoint-lər)
│   ├── Services/        Biznes məntiqi (ITodoService / TodoService)
│   ├── Repositories/    Data access (ITodoRepository / TodoRepository)
│   ├── Data/            AppDbContext (EF Core + PostgreSQL)
│   ├── Entities/        BaseEntity, TodoItem
│   ├── Dtos/            GetTodoDto, PostTodoDto, PutTodoDto
│   ├── Exceptions/      NotFoundException, BadRequestException
│   └── Middlewares/     GlobalExceptionHandlingMiddleware
└── frontend/
    └── src/
        ├── api/todoApi.js        Backend ilə əlaqə (fetch)
        ├── components/           TodoForm, TodoItem
        └── App.jsx
```

## İşə salmaq

**1. Backend + PostgreSQL (Docker ilə — tövsiyə olunur)**

Layihənin kökündə:

```powershell
docker compose up -d --build
```

| Servis | Ünvan |
|--------|-------|
| API | http://localhost:5000 (Swagger: `/swagger`, Health: `/health`) |
| PostgreSQL | `localhost:5432` · DB: `tododb` · User: `todo` · Password: `todo_dev_password` |

Cədvəllər API ilk işə düşəndə avtomatik yaradılır. Data `todo-pgdata` volume-da saxlanılır,
`docker compose down` etsən də itmir (`docker compose down -v` isə volume-u da silir).

**Alternativ: API-ni `dotnet run` ilə, yalnız bazanı Docker-də işlətmək**

```powershell
docker compose up -d db
cd backend/TodoApp.API
dotnet run
```

`appsettings.json`-dakı connection string `localhost:5432`-yə qoşulur.

> `docker run` ilə API-ni əl ilə işə salsan, portu mütləq publish et (`-p 5000:8080`) —
> `-p` olmadan container brauzerdən görünmür və frontend **Failed to fetch** verir.

**2. Frontend** (http://localhost:5173)

```powershell
cd frontend
npm install
npm run dev
```

API ünvanı `frontend/.env` faylında: `VITE_API_URL=http://localhost:5000/api`

## Render-ə deploy (backend + PostgreSQL)

1. Layihəni GitHub-a push et (`render.yaml` repo-nun kökündə olmalıdır).
2. Render dashboard → **New → Blueprint** → repo-nu seç. `render.yaml` həm **todo-db** (PostgreSQL),
   həm də **todo-api** (Docker) servisini yaradır və bazanın bağlantısını API-yə avtomatik ötürür.
3. Soruşulan env dəyəri:
   | Key | Dəyər |
   |-----|-------|
   | `Cors__AllowedOrigins__0` | Frontend ünvanı, məs. `https://todo-app.vercel.app` |
4. Deploy bitəndən sonra: `https://<servis-adı>.onrender.com/swagger`
5. Frontend-də `VITE_API_URL=https://<servis-adı>.onrender.com/api` təyin et.

> Render bağlantını `postgres://user:pass@host/db` formatında verir —
> `Data/ConnectionStringHelper.cs` onu Npgsql formatına çevirir.

> ⚠️ **Render Free plan:** pulsuz PostgreSQL **30 gün** sonra silinir (pullu plana keçmək və ya
> Neon / Supabase kimi pulsuz Postgres istifadə etmək olar — sadəcə `ConnectionStrings__Default`-u dəyiş).
> Free web servis 15 dəq aktivsizlikdən sonra yatır, ilk sorğu ~30–60 san. çəkə bilər.

## API

| Method | Endpoint                  | Təsvir                 |
|--------|---------------------------|------------------------|
| GET    | `/api/todos`              | Bütün tapşırıqlar      |
| GET    | `/api/todos/{id}`         | Bir tapşırıq           |
| POST   | `/api/todos`              | Yeni tapşırıq          |
| PUT    | `/api/todos/{id}`         | Yenilə                 |
| PATCH  | `/api/todos/{id}/toggle`  | Tamamlandı/aktiv dəyiş |
| DELETE | `/api/todos/{id}`         | Sil                    |
