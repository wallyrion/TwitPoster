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