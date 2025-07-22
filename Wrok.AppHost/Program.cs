var builder = DistributedApplication.CreateBuilder(args);

// Parameters
var mediatrLicense = builder.AddParameter("mediatr-license", secret: true);

var jwtExpirationMinutes = builder.AddParameter("jwt-expiration-minutes", "10", secret: false);
var jwtSecret = builder.AddParameter("jwt-secret", "this-should-be-a-super-secret-key-123456789", secret: true);

var dbUser = builder.AddParameter("db-user", "default", secret: false);
var dbPassword = builder.AddParameter("db-password", "default-password", secret: true);

var pg = builder
    .AddAzurePostgresFlexibleServer("postgres")
    .WithPasswordAuthentication(dbUser, dbPassword)
    .RunAsContainer(c => 
        c
            .WithUserName(dbUser)
            .WithPassword(dbPassword)
            .WithHostPort(55432)
            .WithDataVolume()
            .WithPgAdmin());

var identityDb = pg.AddDatabase("wrok-identity-db");

var identityApi = builder
    .AddProject<Projects.Wrok_Identity_Api>("wrok-identity-api")
    .WithEnvironment("Mediatr__License", mediatrLicense)
    .WithEnvironment("Jwt__ExpirationMinutes", jwtExpirationMinutes)
    .WithEnvironment("Jwt__Secret", jwtSecret)
    .WithReference(identityDb).WaitFor(identityDb);

builder.AddProject<Projects.Wrok_Gateway>("wrok-gateway")
    .WithExternalHttpEndpoints()
    .WithReference(identityApi).WaitFor(identityApi);

builder.Build().Run();
