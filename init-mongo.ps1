# Function to wait for a container to be ready
function Wait-ForContainer {
    param (
        [string]$containerName,
        [int]$port
    )

    Write-Host "Waiting for $containerName to be ready on port $port..."
    while (-not (docker exec $containerName mongosh --port $port --eval "print('waiting for $containerName');" 2>$null)) {
        Start-Sleep -Seconds 1
    }
    Write-Host "$containerName is ready."
}

# Wait for the config server to be ready
Wait-ForContainer -containerName "configsvr" -port 27019
# Run the config server initialization script
docker exec configsvr mongosh --port 27019 /scripts/init-configsvr.js

# Wait for shard1 to be ready
Wait-ForContainer -containerName "shard1" -port 27018
# Run the shard1 initialization script
docker exec shard1 mongosh --port 27018 /scripts/init-shard1.js

# Wait for shard2 to be ready
Wait-ForContainer -containerName "shard2" -port 27028
# Run the shard2 initialization script
docker exec shard2 mongosh --port 27028 /scripts/init-shard2.js

# Wait for mongos to be ready
Wait-ForContainer -containerName "mongos" -port 27017
# Run the mongos initialization script
docker exec mongos mongosh --port 27017 /scripts/init-mongos.js

Write-Host "MongoDB sharded cluster initialization complete."
