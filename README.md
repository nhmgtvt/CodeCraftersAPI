# ASP.NET Core API Authentication and Authorization

This API provides authentication and authorization features, supporting both password-based login and OAuth-based external login.

## 🚀 Features

- ✅ User registration with email and password
- ✅ Login using email and password
- ✅ OAuth login with external providers (Google, etc.)
- ✅ Token-based authentication using JWT

## 🛠 Technologies Used

- **ASP.NET Core** for API development
- **JWT (JSON Web Token)** for authentication
- **OAuth 2.0** for external authentication

---


## 🏗 Installation and Setup
-1️⃣ Clone the repository:
git clone https://github.com/your-repo/aspnet-auth-api.git
cd aspnet-auth-api

-2️⃣ Install dependencies:
dotnet restore

-3️⃣ Configure authentication settings in appsettings.json:

-4️⃣ Configure Google OAuth in Google Developer Console:

  Go to Google Cloud Console.
  
  Create a new project or select an existing one.
  
  Navigate to APIs & Services → Credentials.
  
  Click Create Credentials → OAuth 2.0 Client ID.
  
  Configure the consent screen (if not already set up).
  
  Choose Web application as the application type.
  
  Set the Authorized redirect URIs
  
  Click Create, and copy the generated Client ID and Client Secret.

  Add these values to your appsettings.json under the OAuth.Google section.
  

🔑 Authentication

The API uses JWT tokens for authentication.

Upon successful login, a token is returned in the response.

Include the token in the Authorization header when making authenticated requests:
Authorization: Bearer your-jwt-token

## 🏗 Testing

Testing is limited at the moment. For local login, please use swagger. For external login, please start a web server to host TestExternalAuth.html. Note that the file cannot be run directly and also proper settings for external environtment is required.
