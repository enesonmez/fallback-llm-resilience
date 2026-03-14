using FallbackLLMResilience.API.Abstractions;
using FallbackLLMResilience.API.DTOs;
using FallbackLLMResilience.API.Infrastructure;
using FallbackLLMResilience.API.Services;
using FallbackLLMResilience.API.Settings;
using FallbackLLMResilience.API.Validators;
using FluentValidation;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OllamaSharp;
using OpenAI.Chat;

namespace FallbackLLMResilience.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAiInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OpenAiSettings>(configuration.GetSection(nameof(OpenAiSettings)));
        services.Configure<OllamaSettings>(configuration.GetSection(nameof(OllamaSettings)));

        services.AddChatClient(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<FallbackChatClient>>();

            // OpenAI Client
            var openAiSettings = provider.GetRequiredService<IOptions<OpenAiSettings>>().Value;
            var openAiClient = new ChatClient(openAiSettings.Model, openAiSettings.ApiKey).AsIChatClient();

            // Ollama Client
            var ollamaSettings = provider.GetRequiredService<IOptions<OllamaSettings>>().Value;
            var ollamaClient = new OllamaApiClient(new Uri(ollamaSettings.Uri), ollamaSettings.Model);

            return new FallbackChatClient(openAiClient, ollamaClient, logger);
        });

        services.AddValidatorsFromAssemblyContaining<GenerateContentRequestValidator>();
        services.AddScoped<IProductAiService, ProductAiService>();

        return services;
    }
}