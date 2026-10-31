using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace CreditCardApp.Bff.Tests.Support;

public sealed record RecordedRequest(HttpMethod Method, string Path, string? Authorization);

public sealed class StubCoreApi : HttpMessageHandler
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly Dictionary<string, Queue<Func<HttpResponseMessage>>> _routes = [];
    private readonly List<RecordedRequest> _requests = [];
    private readonly object _gate = new();

    public IReadOnlyList<RecordedRequest> Requests
    {
        get
        {
            lock (_gate)
            {
                return _requests.ToList();
            }
        }
    }

    public void Reset()
    {
        lock (_gate)
        {
            _routes.Clear();
            _requests.Clear();
        }
    }

    public void Setup(HttpMethod method, string path, params Func<HttpResponseMessage>[] responses)
    {
        lock (_gate)
        {
            _routes[Key(method, path)] = new Queue<Func<HttpResponseMessage>>(responses);
        }
    }

    public static HttpResponseMessage Json(HttpStatusCode statusCode, object body) =>
        new(statusCode) { Content = JsonContent.Create(body, options: JsonOptions) };

    public static HttpResponseMessage Problem(HttpStatusCode statusCode, string code, string detail) =>
        new(statusCode)
        {
            Content = JsonContent.Create(
                new { title = "Error", status = (int)statusCode, detail, code },
                mediaType: new("application/problem+json"),
                options: JsonOptions)
        };

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Func<HttpResponseMessage>? responder;

        lock (_gate)
        {
            var path = request.RequestUri!.AbsolutePath;
            _requests.Add(new RecordedRequest(request.Method, path, request.Headers.Authorization?.ToString()));

            if (!_routes.TryGetValue(Key(request.Method, path), out var queue) || queue.Count == 0)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            responder = queue.Count > 1 ? queue.Dequeue() : queue.Peek();
        }

        return Task.FromResult(responder());
    }

    protected override void Dispose(bool disposing)
    {
    }

    private static string Key(HttpMethod method, string path) => $"{method} {path}";
}
