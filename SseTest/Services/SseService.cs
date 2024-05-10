using System.Collections.Concurrent;

namespace SseTest.Services;

/// <inheritdoc cref="ISseService"/>
public class SseService : ISseService
{
    private static readonly ConcurrentDictionary<string, SseClient> __clients = new();
    private static readonly ConcurrentDictionary<string, HashSet<string>> __groups = new();

    /// <inheritdoc/>
    public async Task RegisterClientAsync(string id, HttpResponse response, string? groupName = null)
    {
        SseClient client = new(id, response);
        if (__clients.TryAdd(id, client))
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                var clients = __groups.GetOrAdd(groupName, _ => []);
                clients.Add(id);
            }

            try
            {
                await client.KeepAliveAsync();
            }
            catch
            {
                DisconnectClient(id);
                throw;
            }
        }
    }

    /// <inheritdoc/>
    public bool TryGetClient(string clientId, out SseClient? client)
    {
        return __clients.TryGetValue(clientId, out client);
    }

    /// <inheritdoc/>
    public async Task SendEventAsync<T>(IEnumerable<string> clientIds, T? data = null, string? eventType = "message")
        where T : class
    {
        var tasks = __clients.Where(c => clientIds.Contains(c.Key))
                             .Select(c => c.Value.SendAsync(data, eventType));

        await Task.WhenAll(tasks);
    }

    /// <inheritdoc/>
    public async Task SendEventToGroupAsync<T>(string groupName, T? data = null, string? eventType = "message")
        where T : class
    {
        if (__groups.TryGetValue(groupName, out var clientIds))
        {
            await SendEventAsync(clientIds, data, eventType);
        }
    }

    /// <inheritdoc/>
    public bool DisconnectClient(string clientId)
    {
        if (__clients.TryRemove(clientId, out var client))
        {
            foreach (var group in __groups)
            {
                if (group.Value.Remove(clientId) && group.Value.Count == 0)
                {
                    __groups.TryRemove(group.Key, out _);
                }
            }

            client.Disconnect();
            return true;
        }

        return false;
    }
}
