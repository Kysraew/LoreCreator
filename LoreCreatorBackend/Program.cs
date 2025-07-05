using LoreCreatorBackend.Endpoints;
using LoreCreatorBackend.Infrastrucure.Database;
using LoreCreatorBackend.Infrastrucure.LlmCommunication;
using LoreCreatorBackend.Infrastrucure.LlmCommunication.Ollama;
using LoreCreatorBackend.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5173");
                      });
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<LoreDbContext>(opts =>
{
    opts.UseNpgsql(
        builder.Configuration["ConnectionStrings:LoreDbConnection"]);
});

builder.Services.AddSingleton<ILlmProvider, OllamaProvider>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

WorldEndpoints.MapWorldEndpoints(app);


app.Run();

