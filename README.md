# essence
Time is the fire in which we burn

## Run the PostgreSQL & WebAPI database inside Docker
Create `.env` file with following content in root of the repository
```
POSTGRES_USER=essence
POSTGRES_PASSWORD=essence
POSTGRES_DB=essencepg
Essence__PostgreConnection__ConnectionString=Server=database;Port=5432;Database=essencepg;User Id=essence;Password=essence
```

Common operations
* Start the database `docker-compose up`
* Delete it `docker-compose down`
* Check runtime logs `docker-compose logs`
