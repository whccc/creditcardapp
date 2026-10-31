using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace CreditCardApp.Bff.WebApi.Clients;

public abstract class CoreApiClient(HttpClient httpClient)
{
    private const string ProblemJsonMediaType = "application/problem+json";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    protected Task<DownstreamResult<T>> GetAsync<T>(string uri, CancellationToken cancellationToken) =>
        SendAsync<T>(new HttpRequestMessage(HttpMethod.Get, uri), cancellationToken);

    protected Task<DownstreamResult<T>> PostAsync<T>(string uri, object? body, CancellationToken cancellationToken) =>
        SendAsync<T>(new HttpRequestMessage(HttpMethod.Post, uri) { Content = ToJson(body) }, cancellationToken);

    protected Task<DownstreamResult<T>> PutAsync<T>(string uri, object body, CancellationToken cancellationToken) =>
        SendAsync<T>(new HttpRequestMessage(HttpMethod.Put, uri) { Content = ToJson(body) }, cancellationToken);

    protected async Task<DownstreamResult> DeleteAsync(string uri, CancellationToken cancellationToken)
    {
        using var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri), cancellationToken);

        return response.IsSuccessStatusCode
            ? DownstreamResult.Success()
            : DownstreamResult.Failure(await ReadProblemAsync(response, cancellationToken));
    }

    private async Task<DownstreamResult<T>> SendAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using var response = await httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return DownstreamResult<T>.Failure(await ReadProblemAsync(response, cancellationToken));
        }

        var value = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException($"The core API returned an empty body for {request.Method} {request.RequestUri}.");

        return DownstreamResult<T>.Success(value);
    }

    private static async Task<ProblemDetails> ReadProblemAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var statusCode = (int)response.StatusCode;

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status502BadGateway,
                Title = "Error en un servicio interno",
                Detail = "Un servicio interno no pudo procesar la solicitud. Intenta nuevamente en unos minutos.",
                Extensions = { ["code"] = "Bff.DownstreamError" }
            };
        }

        if (response.Content.Headers.ContentType?.MediaType == ProblemJsonMediaType)
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(JsonOptions, cancellationToken);
            if (problem is not null)
            {
                problem.Status ??= statusCode;
                return problem;
            }
        }

        return new ProblemDetails { Status = statusCode, Title = response.ReasonPhrase };
    }

    private static JsonContent? ToJson(object? body) => body is null ? null : JsonContent.Create(body, options: JsonOptions);
}
