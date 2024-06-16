using System.Net.Http.Json;
using FluentAssertions;
using FluentAssertions.Web;

namespace TwitPoster.IntegrationTests.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async Task<T?> GetJsonResponse<T>(this AndConstraint<HttpResponseMessageAssertions> andConstrain)
    {
        return await andConstrain.And.Subject.Content.ReadFromJsonAsync<T>();
    }
}