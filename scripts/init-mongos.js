print("Initializing Mongos Router...");
sh.addShard("shard1ReplSet/shard1:27018");
sh.addShard("shard2ReplSet/shard2:27028");
sh.enableSharding("ChatDb");
sh.shardCollection("ChatDb.Messages", { chatRoomId: 1 });
print("Mongos Router Initialized.");