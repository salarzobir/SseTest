using System.Text.Json;

namespace SseTest.Services;

/// <summary>
/// Represents an SSE client, handling the connection, message sending, and disconnection.
/// </summary>
public class SseClient
{
    private const string __contentTypeEventStreamHeaderValue = "text/event-stream";
    private const string __keepAliveMessage = "data: KEEPALIVE\n\n";

    private readonly HttpResponse _response;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private static readonly JsonSerializerOptions WebSerializerOptions = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Gets the unique identifier for the client.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SseClient"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the client.</param>
    /// <param name="response">The HttpResponse object to send events to.</param>
    public SseClient(string id, HttpResponse response)
    {
        Id = id;
        _response = response;
        _response.StatusCode = StatusCodes.Status200OK;
        _response.ContentType = __contentTypeEventStreamHeaderValue;
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_response.HttpContext.RequestAborted);
    }

    /// <summary>
    /// Starts sending keep-alive messages to maintain the connection.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation of sending keep-alive messages.</returns>
    public async Task KeepAliveAsync()
    {
        while (true)
        {
            await _response.WriteAsync(__keepAliveMessage, _cancellationTokenSource.Token);
            await _response.Body.FlushAsync(_cancellationTokenSource.Token);
            await Task.Delay(5000, _cancellationTokenSource.Token);
        }
    }

    /// <summary>
    /// Sends a message to the client.
    /// </summary>
    /// <typeparam name="T">The type of the message data.</typeparam>
    /// <param name="data">The data to send.</param>
    /// <param name="eventType">The type of the event. Defaults to "message".</param>
    /// <returns>A Task representing the asynchronous operation of sending the message.</returns>
    public async Task SendAsync<T>(T? data = null, string? eventType = "message")
        where T : class
    {
        await _response.WriteAsync($"event: {eventType}\ndata: {(data is not null ? JsonSerializer.Serialize(data, WebSerializerOptions) : string.Empty)}\n\n", _cancellationTokenSource.Token);
        await _response.Body.FlushAsync(_cancellationTokenSource.Token);
    }

    /// <summary>
    /// Disconnects the client, stopping any further messages and cleaning up resources.
    /// </summary>
    public void Disconnect()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }
}
