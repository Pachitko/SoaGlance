DOCKER_COMPOSE_BUILD = docker compose -f docker-compose.yml build
DOCKER_COMPOSE_BUILD_PROD = docker compose -f docker-compose.prod.yml build

DOCKER_COMPOSE_UP = docker compose -f docker-compose.yml --env-file .env.dev up --build -d
DOCKER_COMPOSE_UP_PROD = docker compose -f docker-compose.prod.yml up --build -d

DOCKER_COMPOSE_DOWN = docker compose  --env-file .env.dev down
DOCKER_COMPOSE_LOGS = docker compose logs -f
DOCKER_DROP_DB = docker volume rm idsrv_db

DOTNET_PUBLISH = dotnet publish -c Release src/MsaGlance.sln

IDENTITY_API = Identity.Api
IDENTITY_PROJECT = ./src/Services/Identity/${IDENTITY_API}/${IDENTITY_API}.csproj
DISK_API = Disk.Api
DISK_PROJECT = ./src/Services/Disk/${DISK_API}/${DISK_API}.csproj

pgadmin:
	docker run --rm -d -p 8080:80 --name pgadmin -v pgadmin:/var/lib/pgadmin -e PGADMIN_DEFAULT_EMAIL="a@a.aa" -e PGADMIN_DEFAULT_PASSWORD="b" dpage/pgadmin4:6.9
dropdb:
	$(DOCKER_DROP_DB)

prune:
	docker system prune
publish_docker:

build:
	$(DOCKER_COMPOSE_BUILD)
build-prod:
	$(DOCKER_COMPOSE_BUILD_PROD)

up:
	$(DOCKER_COMPOSE_UP)
reup:
	docker compose -f docker-compose.yml up -d --no-deps --build $(s)
up-prod:
	$(DOCKER_COMPOSE_UP_PROD)

down:
	$(DOCKER_COMPOSE_DOWN)
logs:
	$(DOCKER_COMPOSE_LOGS) $(s)
restart: down up