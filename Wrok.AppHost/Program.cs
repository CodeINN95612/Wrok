var builder = DistributedApplication.CreateBuilder(args);

var pg = builder
    .AddPostgres("postgres")
    .WithHostPort(55432)
    .WithDataVolume()
    .WithPgAdmin();

var identityDb = pg.AddDatabase("wrok-identity-db");

var identityApi = builder
    .AddProject<Projects.Wrok_Identity_Api>("wrok-identity-api")
    .WithReference(identityDb).WaitFor(identityDb);

builder.AddProject<Projects.Wrok_Gateway>("wrok-gateway")
    .WithExternalHttpEndpoints()
    .WithReference(identityApi).WaitFor(identityApi);

builder.Build().Run();
