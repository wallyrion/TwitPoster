﻿using TwitPoster.IntegrationTests.ExternalApis.FakeResponses;
using WireMock;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace TwitPoster.IntegrationTests.ExternalApis;

public class LocationApiServer : IDisposable
{
    public readonly WireMockServer Server = WireMockServer.Start();
    public string Url => Server.Url!;

    public void SetupCountries()
    {
        Server.Given(Request.Create()
                .WithPath($"/api/v0.1/countries/flag/unicode")
                .UsingGet() 
            )
            .RespondWith(Response.Create()
                .WithBody(CountriesResponseJson.All)
                .WithHeader("content-type", "application/json; charset=utf-8"));
    }
    
    public void SetupCountries(Func<IRequestMessage, ResponseMessage> callback)
    {
        Server.Given(Request.Create()
                .WithPath($"/api/v0.1/countries/flag/unicode")
                .UsingGet()
            )
            .RespondWith(Response.Create()
                .WithCallback(callback));
    }
    
    public void Dispose()
    {
        Server.Dispose();
    }
}