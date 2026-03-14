using System.Runtime.CompilerServices;
using Microsoft.Extensions.AI;

namespace FallbackLLMResilience.API.Infrastructure;

public class FallbackChatClient(IChatClient primary, IChatClient fallback, ILogger<FallbackChatClient> logger)
    : IChatClient
{
    public void Dispose()
    {
        primary.Dispose();
        fallback.Dispose();
    }

    public async Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            logger.LogInformation("Attempting to get response from primary AI provider.");
            return await primary.GetResponseAsync(messages, options, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Primary AI provider failed. Switching to fallback AI service.");
            return await fallback.GetResponseAsync(messages, options, cancellationToken);
        }
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        bool primaryFailed = false;

        var enumerator = primary.GetStreamingResponseAsync(messages, options, cancellationToken)
            .GetAsyncEnumerator(cancellationToken);

        try
        {
            while (true)
            {
                ChatResponseUpdate update;
                try
                {
                    if (!await enumerator.MoveNextAsync()) break;
                    update = enumerator.Current;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Primary AI stream interrupted or failed to start. Switching to fallback.");
                    primaryFailed = true;
                    break;
                }

                yield return update;
            }
        }
        finally
        {
            await enumerator.DisposeAsync();
        }

        if (primaryFailed)
        {
            await foreach (var update in fallback.GetStreamingResponseAsync(messages, options, cancellationToken))
            {
                yield return update;
            }
        }
    }

    public object? GetService(Type serviceType, object? serviceKey = null) =>
        primary.GetService(serviceType, serviceKey);
}