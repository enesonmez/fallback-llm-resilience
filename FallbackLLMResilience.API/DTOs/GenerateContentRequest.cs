namespace FallbackLLMResilience.API.DTOs;

public record GenerateContentRequest(string ProductName, List<ProductFeature> Features, string Tone = "Professional");