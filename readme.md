
## Links
### https://twitposter.site 
### [https://twitposter-production-appservice.azurewebsites.net/swagger/index.html](Backend)
Twitposter is a social network platform designed to allow users to connect, share posts, and engage with content.

### Features

- User Authentication
- Register
- Login
- Login with Google
- Post Management
- Create Post
- Get Posts
- Comment Management
- Create Comment
- Like Comment
- User Profile
- View and Change User Profile
- Upload Profile Image

# Technologies Used

## Frontend

You can find link to the frontend code [here](https://github.com/wallyrion/TwitPoster.UI)


## Infrastructure
- GitHub Actions for Continuous Integration and Continuous Deployment with trunk-based development
- Continuous Deployment pipeline (includes Terraform infrastructure and E2E tests)
- Easy to work with environments (1 minute to deploy to a new environment)
- Separate CI pipelines (with dotnet build, integration tests)

## Backend
- ASP.NET Core for the main application
- Azure App Service: Hosts the main application.
- Microservices: Separate service for email sending.
- Azure Blob Storage: Stores profile images.
- Azure Functions: Trigger on blob to compress image and create thumbnail.
- Azure Service Bus: Communicates between services.
- Application Insights: Logging and tracing.
- MS SQL Server: Database server.
- MS SQL Database: Stores application data.
