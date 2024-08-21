# dotnet-logging
A project to demo the effective logging in .NET microservice

## Run MSSQL Database, Elasticsearch, Filebeat and Kibana
If you are using WSL, copy the filebeat.yml to WSL path to let the file belong to root:
```
sudo cp filebeat.yml /filebeat.yml
```

Start all the services 
```
docker-compose -f docker-compose.yaml -- up -d
```

After running above services, it is required to set up index pattern in Kibana:
- Write some logs from application
- Browse [Elasticsearch Index](http://localhost:9200/_cat/indices?v) to make sure there is `filebeat-*` index
- Go to [Kibana](http://localhost:5601) > Menu > Stack Management
- Choose `Index Patterns` under Kibana
- Create an index pattern with pattern filebeat*
- Select `@timestamp` to be Timestamp field
- Click `Create index pattern` button to save