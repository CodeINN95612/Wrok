using Wrok.Identity.Api.Endpoints.Auth;
using Wrok.Identity.Api.Endpoints.Invitations;
using Wrok.Identity.Api.Endpoints.Users;
using Wrok.Identity.Application.Extensions;
using Wrok.Identity.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication(builder.Configuration);

builder.Services.AddProblemDetails();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseInfrastructureAuth();

app.MapDefaultEndpoints();

app.MapRegisterEndpoint();
app.MapLoginEndpoint();
app.MapRefreshTokenEndpoint();

app.MapGetAllUsersEndpoint();
app.MapGetUserByIdEndpoint();

app.MapInviteEndpoint();
app.MapAcceptInviteEndpoint();

app.Run();  