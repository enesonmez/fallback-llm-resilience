using System.Runtime.CompilerServices;
using FallbackLLMResilience.API.Abstractions;
using FallbackLLMResilience.API.DTOs;
using Microsoft.Extensions.AI;

namespace FallbackLLMResilience.API.Services;

public class ProductAiService(IChatClient chatClient) : IProductAiService
{
    public async IAsyncEnumerable<string> StreamDescriptionAsync(GenerateContentRequest request, [EnumeratorCancellation] CancellationToken ct)
    {
        var systemMessage = new ChatMessage(ChatRole.System, """
                                                             Sen profesyonel bir e-ticaret içerik editörüsün. 
                                                             Sana verilen teknik özellikleri kullanarak ikna edici ürün açıklamaları yazarsın.
                                                             Format: Sadece düz metin dön, Markdown kullanma.
                                                             """);

        var featureString = string.Join(", ", request.Features.Select(f => $"{f.Name}: {f.Value}"));
        var userMessage = new ChatMessage(ChatRole.User,
            $"Ürün Adı: {request.ProductName}. Özellikler: {featureString}. Ton: {request.Tone} Bu ürün için 300 kelimelik açıklama yaz.");

        var responseStream = chatClient.GetStreamingResponseAsync([systemMessage, userMessage], cancellationToken: ct);

        await foreach (var message in responseStream)
        {
            yield return message.Text ?? string.Empty;
        }
    }
}