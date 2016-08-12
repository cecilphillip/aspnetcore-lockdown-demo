# Code samples on Securing ASP.NET Core

Getting Setup
-------------
The samples are built with ASP .NET Core 1.0 RTM with tooling version 1.0.0-preview2-003121. You can get the installation
files [here](https://get.asp.net). There's nothing OS specific in here so the code *should* run anywhere ASP.NET Core is supported.
The code also assumes that you have a GitHub app setup. You can create a GitHub application for free [here](https://github.com/settings/applications/new).
Then add your keys to the secrets manager with the following commands 
```
dotnet user-secrets set GitHub:ClientId YOUR_SECRET
dotnet user-secrets set GitHub:SecretKey YOUR_KEY
```
Covered in the samples
-------------

* Cookie Authentication Middleware
* GitHub OAuth Middleware
* ClaimsTransformation
* TagHelpers
* Authorization Policies
* User Secrets
* Entity Framework InMemory storage (for demos/testing only)

#### Built With
- [ASP.NET Core](http://www.asp.net/core)
- [Entity Framework Core](https://docs.efproject.net)
- [ASP .NET Contrib](https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers)