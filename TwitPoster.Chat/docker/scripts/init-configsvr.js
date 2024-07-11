print("Initializing Config Server...");
rs.initiate({
    _id: "configReplSet",
    configsvr: true,
    members: [{ _id: 0, host: "configsvr:27019" }]
});
print("Config Server Initialized.");