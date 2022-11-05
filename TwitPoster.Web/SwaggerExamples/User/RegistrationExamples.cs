using Bogus;
using Swashbuckle.AspNetCore.Filters;
using TwitPoster.Web.ViewModels;
// ReSharper disable UnusedType.Global

namespace TwitPoster.Web.SwaggerExamples.User;

public class RegistrationRequestExamples : IMultipleExamplesProvider<RegistrationRequest>
{
    public IEnumerable<SwaggerExample<RegistrationRequest>> GetExamples()
    {
        return Enumerable.Range(0, 10)
            .Select(number => SwaggerExample.Create($"Example {number}", GetExampleWithBogus()))
            .ToList();
    }

    private static RegistrationRequest GetExampleWithBogus()
    {
        var faker = new Faker();
            
        return new RegistrationRequest(faker.Person.FirstName, faker.Person.LastName, faker.Person.DateOfBirth, faker.Person.Email, faker.Internet.Password());
    }
}

public class RegistrationResponseExamples : IMultipleExamplesProvider<RegistrationResponse>
{
    const string ExampleToken =
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI0OSIsImdpdmVuX25hbWUiOiJKb2huIiwiZmFtaWx5X25hbWUiOiJEb2UiLCJqdGkiOiJmZmUxZWEzMy0wZDg1LTQxZDMtODZiMS1mNDdiMjg2NGY0MjkiLCJleHAiOjE2Njc2NDc4NDYsImlzcyI6IlR3aXRQb3N0ZXIuV2ViIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwLyJ9.iVJRAy_aEhXrQej-362EmORAzly-aCpCSt8acHMFy2E";

    public IEnumerable<SwaggerExample<RegistrationResponse>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "Example 1",
            new RegistrationResponse(2, ExampleToken));
    }
}
