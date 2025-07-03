var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Wrok_Identity_Api>("wrok-identity-api");

builder.Build().Run();
