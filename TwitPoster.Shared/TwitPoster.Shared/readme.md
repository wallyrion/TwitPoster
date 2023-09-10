# TwitPoster Shared Library

This is the shared library for the TwitPoster project, which consists of common code used throughout the project.

Here are some instructions to manage the NuGet package for this library:

## Creating a NuGet Package

An utility is needed that packs the shared library into a NuGet package for distribution.
`dotnet pack`


## Publishing the NuGet Package

To publish the created package to NuGet.org, you must have your API key. Use the following command, substituting `<apikey>` with your actual API key:

`dotnet nuget push twitposter-shared.1.0.0.nupkg --api-key <apikey> --source https://api.nuget.org/v3/index.json`

In the command above, pay attention to the 1.0.0 which signifies the version of the package being pushed.

