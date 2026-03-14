using FallbackLLMResilience.API.DTOs;

namespace FallbackLLMResilience.API.Abstractions;

public interface IProductAiService
{
    IAsyncEnumerable<string> StreamDescriptionAsync(GenerateContentRequest request, CancellationToken ct);
}