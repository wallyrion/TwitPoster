print("Initializing Shard2...");
rs.initiate({
    _id: "shard2ReplSet",
    members: [{ _id: 0, host: "shard2:27028" }]
});
print("Shard2 Initialized.");