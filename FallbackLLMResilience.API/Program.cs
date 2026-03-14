using FallbackLLMResilience.API.Abstractions;
using FallbackLLMResilience.API.DTOs;
using FallbackLLMResilience.API.Extensions;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Extensions
builder.Services.AddAiInfrastructure(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapPost("/api/products/generate-description", async (
        GenerateContentRequest request, 
        IValidator<GenerateContentRequest> validator,
        IProductAiService aiService, 
        CancellationToken ct) =>
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());
        
        return Results.Ok(aiService.StreamDescriptionAsync(request, ct));
    })
    .WithName("GenerateProductDescription")
    .WithOpenApi();

app.Run();
