print("Initializing Shard1...");
rs.initiate({
    _id: "shard1ReplSet",
    members: [{ _id: 0, host: "shard1:27018" }]
});
print("Shard1 Initialized.");