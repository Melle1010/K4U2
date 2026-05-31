# K4U2 - LLM Proxy & Content API

A dual-API microservice architecture for managing LLM interactions and message persistence. The **LLM Proxy API** interfaces with local Ollama models, while the **Content API** handles message storage and orchestration.

---

## Table of Contents

- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Installation & Setup](#installation--setup)
- [User Secrets Configuration](#user-secrets-configuration)
- [Configuration Files](#configuration-files)
- [Running the Applications](#running-the-applications)
- [API Endpoints](#api-endpoints)
- [Development](#development)
- [Troubleshooting](#troubleshooting)
- [Security](#security)

---

## Project Structure

```
K4U2/
├── LLM Proxy API/           # ASP.NET Core API for Ollama LLM integration
│   ├── Controllers/
│   │   └── AiController.cs
│   ├── Middlewares/
│   │   ├── ApiKeyMiddleware.cs
│   │   └── ExceptionMiddleware.cs
│   ├── DTOs/
│   ├── Properties/
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── Program.cs
│
├── Content API/             # ASP.NET Core API for message persistence
│   ├── Controllers/
│   │   └── MessagesController.cs
│   ├── Data/
│   │   └── AppDbContext.cs
│   ├── Models/
│   ├── DTOs/
│   ├── Middlewares/
│   ├── Properties/
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── Program.cs
│
├── evaluation.md            # Quality evaluation report
└── README.md               # This file
```

---

## Prerequisites

### System Requirements
- **.NET 10 SDK** or later ([Download](https://dotnet.microsoft.com/download/dotnet/10.0))
- **Visual Studio 2026 Community** (or VS Code with C# extension)
- **Ollama** (for LLM functionality) ([Download](https://ollama.ai))
- **.NET user-secrets** tool (included with .NET SDK)

### Dependencies

**LLM Proxy API:**
- OllamaSharp (for Ollama integration)
- Microsoft.AspNetCore.OpenApi
- Scalar.AspNetCore (for API documentation)

**Content API:**
- Microsoft.EntityFrameworkCore (In-Memory database for development)
- Microsoft.AspNetCore.OpenApi
- Scalar.AspNetCore

---

## Installation & Setup

### 1. Clone the Repository

```powershell
git clone https://github.com/Melle1010/K4U2.git
cd K4U2
```

### 2. Restore NuGet Packages

```powershell
# Restore all packages
dotnet restore
```

### 3. Verify .NET Version

```powershell
dotnet --version
# Should output: 10.x.x or later
```

### 4. Set Up Ollama (Required for LLM Proxy API)

```powershell
# Download and install Ollama from https://ollama.ai
# Then pull a model (examples):

ollama pull gemma2:7b    # Recommended (faster)
ollama pull gemma3:4b    # Currently hardcoded in AiController
ollama pull llama2:7b    # Alternative

# Start Ollama service
ollama serve             # On Windows, Ollama runs as system service by default
```

**Note:** By default, Ollama listens on `http://localhost:11434`. The project uses `https://ollama.com` as configured URI (ensure this is correct in your environment).

---

## User Secrets Configuration

User secrets safely store sensitive configuration values (API keys, connection strings) outside of source control.

### Initialize User Secrets

**For LLM Proxy API:**

```powershell
cd "LLM Proxy API"

# Initialize user secrets (creates secret store)
dotnet user-secrets init

# Verify initialization
dotnet user-secrets list
```

**For Content API:**

```powershell
cd "..\Content API"

# Initialize user secrets
dotnet user-secrets init

# Verify initialization
dotnet user-secrets list
```

### Set Required Secrets

#### LLM Proxy API Secrets

```powershell
cd "LLM Proxy API"

# Set Ollama API Key (adjust if using cloud Ollama)
dotnet user-secrets set "OllamaApiKey" "your-ollama-api-key-here"

# Set API Key for Content API communication
dotnet user-secrets set "ApiKey" "your-api-key-here"

# Verify secrets are set
dotnet user-secrets list
```

#### Content API Secrets

```powershell
cd "..\Content API"

# Set API Key for LLM Proxy API communication
dotnet user-secrets set "ApiKey" "your-api-key-here"

# Set LLM Proxy base URL (optional, defaults to http://localhost:5118/)
dotnet user-secrets set "LlmProxy:BaseUrl" "http://localhost:5118/"

# Verify secrets are set
dotnet user-secrets list
```

### User Secrets Storage Location

User secrets are stored in a JSON file at:

**Windows:**
```
%APPDATA%\Microsoft\UserSecrets\<user-secrets-id>\secrets.json
```

**macOS/Linux:**
```
~/.microsoft/usersecrets/<user-secrets-id>/secrets.json
```

To find your `user-secrets-id`, check `.csproj` file:

```xml
<PropertyGroup>
    <UserSecretsId>your-unique-id-here</UserSecretsId>
</PropertyGroup>
```

### Clear/Remove Secrets

```powershell
# Clear all secrets for a project
dotnet user-secrets clear

# Remove a specific secret
dotnet user-secrets remove "OllamaApiKey"
```

---

## Configuration Files

### appsettings.json

**LLM Proxy API** - `LLM Proxy API/appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

**Content API** - `Content API/appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### appsettings.Development.json

Used during development with additional debug logging:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

### Environment Variables

Create a `.env` file in the project root (optional):

```env
ASPNETCORE_ENVIRONMENT=Development
LLM_PROXY_PORT=5118
CONTENT_API_PORT=5024
OLLAMA_URL=http://localhost:11434
```

Load in PowerShell:

```powershell
Get-Content .env | ForEach-Object {
    $key, $value = $_ -split '=', 2
    [Environment]::SetEnvironmentVariable($key, $value)
}
```

---

## Running the Applications

### Option 1: Run Both APIs from Visual Studio

1. Open `K4U2.sln` in Visual Studio 2026
2. Right-click on Solution → **Set Startup Projects**
3. Select **Multiple startup projects**
4. Set both **LLM Proxy API** and **Content API** to **Start**
5. Press **F5** or **Ctrl+F5**

### Option 2: Run from Command Line

**Terminal 1 - LLM Proxy API:**

```powershell
cd "LLM Proxy API"
dotnet run
# Runs on https://localhost:5118 (HTTPS) or http://localhost:5118 (HTTP)
```

**Terminal 2 - Content API:**

```powershell
cd "Content API"
dotnet run
# Runs on https://localhost:5024 (HTTPS) or http://localhost:5024 (HTTP)
```

### Verify APIs are Running

```powershell
# Test LLM Proxy API
curl -I http://localhost:5118/api/ai

# Test Content API
curl -I http://localhost:5024/api/messages
```

### Access API Documentation

After starting the APIs, visit:

- **LLM Proxy API Docs:** `https://localhost:5118/scalar/v1` or `https://localhost:5118/openapi/v1.json`
- **Content API Docs:** `https://localhost:5024/scalar/v1` or `https://localhost:5024/openapi/v1.json`

---

## API Endpoints

### LLM Proxy API

#### Ask and Save
- **Method:** `POST`
- **Endpoint:** `/api/ai/ask-and-save`
- **Authentication:** `X-API-KEY` header required
- **Request Body:** 
  ```json
  "Your question here"
  ```
- **Response:**
  ```json
  {
    "Status": "Saved to Content API",
    "Data": "AI response here..."
  }
  ```
- **Example:**
  ```powershell
  $headers = @{"X-API-KEY" = "your-api-key-here"}
  $body = '"What is the capital of France?"' | ConvertTo-Json
  Invoke-WebRequest -Uri "http://localhost:5118/api/ai/ask-and-save" `
    -Method Post -Headers $headers -Body $body -ContentType "application/json"
  ```

### Content API

#### Get All Messages
- **Method:** `GET`
- **Endpoint:** `/api/messages`
- **Query Parameters:**
  - `startDate` (optional): Filter by date
  - `sort` (optional): `asc` or `desc` (default: `asc`)
- **Response:** Array of Message objects

#### Create Message
- **Method:** `POST`
- **Endpoint:** `/api/messages/create-message`
- **Request Body:**
  ```json
  {
    "text": "Message content"
  }
  ```
- **Response:** Created Message object with ID

#### Update Message
- **Method:** `PUT`
- **Endpoint:** `/api/messages/{id}`
- **Request Body:**
  ```json
  {
    "text": "Updated message content"
  }
  ```

#### Delete Message
- **Method:** `DELETE`
- **Endpoint:** `/api/messages/{id}`
- **Response:** 204 No Content

#### Send Prompt to AI
- **Method:** `POST`
- **Endpoint:** `/api/messages/send-a-prompt-to-ai-model`
- **Request Body:** 
  ```json
  "Your prompt here"
  ```
- **Response:** AI response with prompt included

---

## Development

### Project Configuration

**LLM Proxy API** uses the following configuration in `Program.cs`:

```csharp
var ollamaUrl = new Uri("https://ollama.com");
var apiKey = builder.Configuration["OllamaApiKey"] ?? throw new Exception("Ollama API key is not configured.");

builder.Services.AddScoped<IOllamaApiClient>(sp =>
{
    var httpClient = new HttpClient { BaseAddress = ollamaUrl };
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    return new OllamaApiClient(httpClient);
});

builder.Services.AddHttpClient("ContentApiClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5024/");
});
```

**Content API** uses In-Memory database for development:

```csharp
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseInMemoryDatabase("MyDatabase"));
```

### Running Tests

```powershell
# Run all tests in solution
dotnet test

# Run tests for specific project
dotnet test "LLM Proxy API"
dotnet test "Content API"

# Run with verbose output
dotnet test --verbosity detailed
```

### Building the Solution

```powershell
# Debug build
dotnet build

# Release build
dotnet build --configuration Release

# Clean build
dotnet clean
dotnet build
```

### Debugging in Visual Studio

1. Set breakpoints by clicking on line numbers
2. Press **F5** to start debugging
3. Execution will pause at breakpoints
4. Use **Debug** menu to step through code, evaluate expressions, etc.

### Code Style

The project follows C# conventions:
- PascalCase for class names, methods, and properties
- camelCase for local variables and parameters
- Async methods should end with `Async` suffix
- Use `var` for type inference when obvious

---

## Troubleshooting

### Issue: "API key configuration is missing"

**Cause:** User secret `OllamaApiKey` not set for LLM Proxy API

**Solution:**
```powershell
cd "LLM Proxy API"
dotnet user-secrets set "OllamaApiKey" "your-key"
```

---

### Issue: "API-nyckel saknas" (API key missing) on request

**Cause:** Missing `X-API-KEY` header in request

**Solution:** Add header to all requests to LLM Proxy API:
```powershell
$headers = @{"X-API-KEY" = "your-api-key-here"}
Invoke-WebRequest -Uri "..." -Headers $headers
```

---

### Issue: "Error while calling LLM Proxy API" from Content API

**Cause:** LLM Proxy API not running or wrong port

**Solution:**
1. Verify LLM Proxy API is running: `dotnet run` in `LLM Proxy API/`
2. Check port configuration (default: 5118)
3. Ensure `X-API-KEY` header is set correctly

---

### Issue: Ollama connection timeout

**Cause:** Ollama service not running or wrong URI

**Solution:**
```powershell
# Start Ollama service
ollama serve

# Or verify it's running
curl http://localhost:11434/api/tags
```

---

### Issue: Port already in use (5118 or 5024)

**Cause:** Another application using the port

**Solution:**
```powershell
# Find process using port (Windows)
netstat -ano | findstr :5118

# Kill process (replace PID with actual process ID)
taskkill /PID <PID> /F

# Or run APIs on different ports
dotnet run --urls "https://localhost:5119"
```

---

### Issue: SSL/TLS certificate errors

**Cause:** Self-signed certificates not trusted (development environment)

**Solution:**
```powershell
# Install development certificate
dotnet dev-certs https --trust

# Or disable HTTPS for development
dotnet run --no-launch-profile
```

---

### Issue: In-Memory database reset on restart

**Expected Behavior:** Content API uses in-memory database; data is lost on restart

**Solution:** For persistent storage, modify `Program.cs`:
```csharp
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite("Data Source=messages.db"));
```

---

## Security

### API Key Management

- **Never** commit API keys to version control
- Always use **User Secrets** in development
- Use **Azure Key Vault** or similar service in production
- Rotate keys regularly
- Use strong, randomly generated keys

### HTTPS/TLS

- Development uses self-signed certificates
- Production should use valid certificates (e.g., Let's Encrypt)
- Enable HSTS (HTTP Strict Transport Security) in production

### Input Validation

- Both APIs validate input against null/whitespace
- Consider adding additional validation for production use:
  - Prompt length limits
  - Prompt injection detection
  - Rate limiting
  - Content filtering

### Logging

- Sensitive data (API keys, full prompts) is NOT logged
- Logs are written to console and file
- Review logs regularly for security issues

### CORS Policy

- Currently, CORS is not explicitly configured
- Add CORS policy in `Program.cs` if accessing from different domain:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("https://your-frontend-domain.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

app.UseCors("AllowSpecificOrigins");
```

---
