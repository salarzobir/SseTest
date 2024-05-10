namespace SseTest.Services;

/// <summary>
/// Manages server-sent events (SSE) for clients, including registration, message sending, and disconnection.
/// </summary>
public interface ISseService
{
    /// <summary>
    /// Registers a new client for SSE, optionally adds them to a group, and starts the keep-alive process.
    /// </summary>
    /// <param name="id">The unique identifier for the client.</param>
    /// <param name="response">The HttpResponse object to send events to.</param>
    /// <param name="groupName">Optional. The name of the group to add the client to.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task RegisterClientAsync(string id, HttpResponse response, string? groupName = null);

    /// <summary>
    /// Tries to get a client by its ID.
    /// </summary>
    /// <param name="clientId">The ID of the client to get.</param>
    /// <param name="client">When this method returns, contains the client associated with the specified ID, if the ID is found; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the client was found; otherwise, <see langword="false"/>.</returns>
    bool TryGetClient(string clientId, out SseClient? client);

    /// <summary>
    /// Sends a message to specified clients.
    /// </summary>
    /// <typeparam name="T">The type of the message data.</typeparam>
    /// <param name="clientIds">The IDs of the clients to send the message to.</param>
    /// <param name="data">The data to send.</param>
    /// <param name="eventType">The type of the event. Defaults to "message".</param>
    /// <returns>A Task representing the asynchronous operation of sending messages.</returns>
    Task SendEventAsync<T>(IEnumerable<string> clientIds, T? data = null, string? eventType = "message")
        where T : class;

    /// <summary>
    /// Sends a message to all clients in a specified group.
    /// </summary>
    /// <typeparam name="T">The type of the message data.</typeparam>
    /// <param name="groupName">The name of the group whose clients will receive the message.</param>
    /// <param name="data">The message data to send.</param>
    /// <param name="eventType">The type of the event. Defaults to "message".</param>
    /// <returns>A Task representing the asynchronous operation of sending messages to the group.</returns>
    Task SendEventToGroupAsync<T>(string groupName, T? data = null, string? eventType = "message")
        where T : class;

    /// <summary>
    /// Disconnects a client and removes it from any groups.
    /// </summary>
    /// <param name="clientId">The ID of the client to disconnect.</param>
    /// <returns><see langword="true"/> if the client was found and disconnected; otherwise, <see langword="false"/>.</returns>
    bool DisconnectClient(string clientId);
}
