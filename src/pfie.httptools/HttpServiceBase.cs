using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OneOf.Types;
using pfie.utilities;
using System.Net;
using System.Net.Http.Json;

namespace pfie.http;

public abstract class HttpServiceBase
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;

    public HttpServiceBase(HttpClient httpClient, ILogger logger)
    {
        _httpClient = httpClient.ThrowIfNull();
        _logger = logger.ThrowIfNull();
    }

    public abstract string ServiceName { get; }

    protected async Task<ResultOf<T>> Get<T>(string url, Func<HttpResponseMessage, string, T>? onSuccess = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            var content = await ReadContent(response);

            if (response.IsSuccessStatusCode)
                return onSuccess is null
                    ? Deserialise<T>(content)
                    : onSuccess(response, content);

            return ParseFailure(response, content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Failure.From(ex);
        }
    }

    protected async Task<Result> Post(string url, object data, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, data, cancellationToken);
            var content = await ReadContent(response);

            if (response.IsSuccessStatusCode)
                return new Success();

            return ParseFailure(response, content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Failure.From(ex);
        }
    }

    protected async Task<ResultOf<T>> Post<T>(string url, object data, Func<HttpResponseMessage, string, T>? onSuccess = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, data, cancellationToken);
            var content = await ReadContent(response);

            if (response.IsSuccessStatusCode)

                return onSuccess is null    
                    ? Deserialise<T>(content)
                    : onSuccess(response, content);

            return ParseFailure(response, content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Failure.From(ex);
        }
    }

    public Failure ParseFailure(HttpResponseMessage response, string content) => response.StatusCode switch
    {
        HttpStatusCode.NotFound => Failure.From(new NotFound()),
        HttpStatusCode.Conflict => Failure.From(new Conflict()),
        HttpStatusCode.BadGateway => Failure.From(new BadGateway($"{ServiceName} returned {response.ReasonPhrase} with content {content}")),
        HttpStatusCode.InternalServerError => Failure.From(new BadGateway($"{ServiceName} returned {response.ReasonPhrase} with content {content}")),
        HttpStatusCode.BadRequest => Failure.From(ParseBadRequestResponse(content)),
        _ => Failure.From(new Exception($"{ServiceName} returned unexpected response code {response.StatusCode}"))
    };

    private BadRequest ParseBadRequestResponse(string content)
    {
        try
        {
            var pd = Deserialise<ValidationProblemDetails>(content);

            if (pd.Succeeded)
                return new BadRequest(pd.Value.Title, pd.Value);

            return new BadRequest("",null);

        }
        catch (Exception)
        {
            return new BadRequest("Details of the request could not be parsed");
        }
    }

    private async Task<string> ReadContent(HttpResponseMessage response)
    {
        try
        {
            return await response.Content.ReadAsStringAsync();

        }
        catch (Exception ex)
        {
            return ex.Message;
        }

    }

    private ResultOf<T> Deserialise<T>(string content)
    {
        try
        {
            var data = JsonConvert.DeserializeObject<T>(content);

            return data is null
                ? Failure.From(new Exception($"Response content could not be mapped to {typeof(T).Name}. Result was null"))
                : data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Failure.From(ex);
        }

    }
}
