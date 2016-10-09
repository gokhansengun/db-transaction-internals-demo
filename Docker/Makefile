.PHONY: build postgres app clean ps

build:
	docker-compose up build

app: build
	# start the postgres database
	docker-compose up -d postgres-db
	# wait for 8 seconds initialization
	echo "Will wait for 8 seconds for PostgreSQL to init"
	sleep 8 
	docker-compose up app

clean:
	docker-compose down -v --remove-orphans
	docker rmi $$(docker images | grep "^mono-transaction" | awk '{print $$3}')

ps:
	docker-compose ps 
