namespace FallbackLLMResilience.API.Settings;

public class OllamaSettings
{
    public string Uri { get; set; } = "http://localhost:11434";
    public string Model { get; set; } = "llama3.2";
}