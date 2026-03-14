namespace FallbackLLMResilience.API.DTOs;

public record GenerateContentResponse(string GeneratedDescription, string MetaTitle, List<string> Keywords);