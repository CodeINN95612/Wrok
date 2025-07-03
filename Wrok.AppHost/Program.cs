var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Wrok_Auth_Api>("wrok-auth-api");

builder.Build().Run();
