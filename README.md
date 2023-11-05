# essence
Time is the fire in which we burn

## Run the PostgreSQL database inside docker
Create `.env` file with following content in root of the repository
```
POSTGRES_USER=essence
POSTGRES_PASSWORD=essence
POSTGRES_DB=essencepg
```

Configure user secrets of Essence.WebAPI project with the connection point to docker
```
{
  "Essence": {
    "PostgreConnection": {
      "ConnectionString": "Server=localhost;Port=5433;Database=essencepg;User Id=essence;Password=essence"
    }
  }
}
```

Common operations
* Start the database `docker-compose up -d --build`
* Delete it `docker-compose down`
* Check runtime logs `docker-compose logs`