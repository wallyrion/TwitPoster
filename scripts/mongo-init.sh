#!/bin/bash

# Function to wait for a container to be ready
wait_for_container() {
    local container_name=$1
    local port=$2

    echo "Waiting for $container_name to be ready on port $port..."
    while ! docker exec $container_name mongosh --port $port --eval "print('waiting for $container_name');"; do
        echo "Still waiting for $container_name..."
        sleep 1
    done
    echo "$container_name is ready."
}

# Wait for the config server to be ready
wait_for_container "configsvr" 27019
# Run the config server initialization script
docker exec configsvr mongosh --port 27019 /scripts/init-configsvr.js

# Wait for shard1 to be ready
wait_for_container "shard1" 27018
# Run the shard1 initialization script
docker exec shard1 mongosh --port 27018 /scripts/init-shard1.js

# Wait for shard2 to be ready
wait_for_container "shard2" 27028
# Run the shard2 initialization script
docker exec shard2 mongosh --port 27028 /scripts/init-shard2.js

# Wait for mongos to be ready
wait_for_container "mongos" 27017
# Run the mongos initialization script
docker exec mongos mongosh --port 27017 /scripts/init-mongos.js

echo "MongoDB sharded cluster initialization complete."
