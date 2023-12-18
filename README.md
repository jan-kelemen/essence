# essence ![Build](https://github.com/jan-kelemen/essence/actions/workflows/main.yaml/badge.svg)
Time is the fire in which we burn

## Run Essence inside Docker
Create `.env` file with following content in root of the repository
```
POSTGRES_USER=essence
POSTGRES_PASSWORD=essence
POSTGRES_DB=essencepg
```

Common operations:
* Start application `docker-compose up`
* Delete it `docker-compose down`
* Check runtime logs `docker-compose logs`
