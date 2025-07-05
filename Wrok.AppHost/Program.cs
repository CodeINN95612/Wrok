var builder = DistributedApplication.CreateBuilder(args);

var pg = builder
    .AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();
var identityDb = pg.AddDatabase("wrok-identity-db");

builder
    .AddProject<Projects.Wrok_Identity_Api>("wrok-identity-api")
    .WithReference(identityDb);

builder.Build().Run();
