# dotnet-logging
A project to demo the effective logging in .NET microservice

## Run MSSQL Database, Elasticsearch, Filebeat and Kibana
If you are using WSL, copy the filebeat.yml to WSL path to let the file belong to root:
```
sudo cp filebeat.yml /filebeat.yml
```

```
docker-compose -f docker-compose.yaml -- up -d
```

