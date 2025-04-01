# 🏗 Architecture

This project follows the Clean Architecture principles, ensuring separation of concerns and high maintainability to minimize changing in code in case of requirements change. The structure is divided into:

## Core Layer: Contains business logic and domain entities
The Core Layer relies solely on .NET standard libraries and does not depend on any external libraries or frameworks. It should contain only pure logic, such as a function that calculates the final price based on the base price and discount.

## Application Layer: Handles use cases and service logic
Use cases include OAuthLoginService, PasswordLoginService, and RegisterService. All dependencies should be defined as abstractions.

## Infrastructure Layer: Manages data access and external services
### Give concretes for dependency abstractions in Application layer. Infrastructure packages are added as below:

Microsoft.EntityFrameworkCore

Microsoft.EntityFrameworkCore.InMemory

Microsoft.EntityFrameworkCore.Sqlite

BCrypt.Net-Next

Microsoft.IdentityModel.Tokens

System.IdentityModel.Tokens.Jwt

Microsoft.Extensions.Configuration.Binder

## Presentation Layer: The API controllers and endpoints
### Packages for ASP.Net Core API:

Microsoft.AspNetCore.Authentication.JwtBearer

Microsoft.AspNetCore.OpenApi

Microsoft.VisualStudio.Azure.Containers.Tools.Targets

Swashbuckle.AspNetCore

