using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SyncPodcast.Application;
using SyncPodcast.API.Middleware;
using SyncPodcast.Infrastructure;
using SyncPodcast.Infrastructure.Authentication;

var builder = WebApplication.CreateBuilder(args);

/// Service Regestration
builder.Services.AddApplication();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

