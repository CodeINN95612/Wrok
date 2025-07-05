var builder = DistributedApplication.CreateBuilder(args);

var dbUser = builder.AddParameter("db-user", secret: true);
var dbPassword = builder.AddParameter("db-pass", secret: true);

var pg = builder.AddPostgres("postgres").WithPgAdmin();
var identityDb = pg.AddDatabase("wrok-identity-db");

builder
    .AddProject<Projects.Wrok_Identity_Api>("wrok-identity-api")
    .WithReference(identityDb);

builder.Build().Run();
