# RebrickableSharp

[![NuGet](https://img.shields.io/nuget/v/RebrickableSharp?color=blue)](https://www.nuget.org/packages/RebrickableSharp/)
[![Build Status](https://dev.azure.com/jeisenbach/RebrickableSharp/_apis/build/status/gebirgslok.RebrickableSharp?branchName=main)](https://dev.azure.com/jeisenbach/RebrickableSharp/_build/latest?definitionId=3&branchName=main)

## Introduction
RebrickableSharp is a strongly-typed, easy-to-use C# client for the [Rebrickable API](https://rebrickable.com/api/v3/docs/) that 
gets you started with just a few lines of code. It handles authentication, error handling and parsing of JSON into typed instances.
It supports all .NET platforms compatible with *.NET standard 2.0*.

## Related projects
- [BricklinkSharp](https://github.com/gebirgslok/BricklinkSharp) - Easy-to-use C# client for the bricklink (LEGO) marketplace API.

## Changelog
- See [CHANGELOG.md](CHANGELOG.md)

## Get Started

### Demo Project
Check out the [demo project](https://github.com/gebirgslok/RebrickableSharp/tree/main/RebrickableSharp.Demos) for full-featured examples.

### Prerequisites
You need to have an account on [Rebrickable](https://www.rebrickable.com/). Then, go to *Account > Settings > API* and create a new **Key** or use an existing one.

Note that the CSV helper classes do not require an API Key to be used.

### Install NuGet Package
#### Package Manager Console
 ```
Install-Package RebrickableSharp
```
#### Command Line
```
nuget install RebrickableSharp
```

### Setup credentials

```csharp    
RebrickableClientConfiguration.Instance.ApiKey = "<Your API Key>";
```

### IRebrickableClient
```csharp  
var client = RebrickableClientFactory.Build();

// Do stuff

// Client must be disposed properly
client.Dispose();
```
Alternatively, an **externally** managed *HttpClient* can be used:
```csharp
var httpClient = new HttpClient();
var client = RebrickableClientFactory.Build(httpClient);

// Do stuff

// Client *and* HttpClient must be disposed properly
client.Dispose();
httpClient.Dispose();
```

#### Usage recommendation
It's recommended to create and use one *IRebrickableClient* client throughout the lifetime of your application.

In applications using an IoC container you may register the *IRebrickableClient* as a service and inject it into consuming instances (e.g. controllers).
See the below examples to register the *IRebrickableClient* as single instance (Singleton).
	
#### [Autofac](https://autofac.org/) example
```csharp
containerBuilder.Register(c => RebrickableClientFactory.Build())
	.As<IRebrickableClient>()
	.SingleInstance();
```

#### [Microsoft.Extensions.DependencyInjection](https://docs.microsoft.com/de-de/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-5.0) example
```csharp
services.AddSingleton(typeof(IRebrickableClient), provider =>
{
    return RebrickableClientFactory.Build();
});  
``` 

### Parts

####  Get parts

```csharp
// API
Task<PagedResponse<Part>> GetPartsAsync(int page = 1, int pageSize = 100, 
    bool includeDetails = false, string? bricklinkId = null,
    string? partNumber = null, IEnumerable<string>? partNumbers = null,
    int? categoryId = null, string? brickOwlId = null,
    string? legoId = null, string? lDrawId = null,
    string? searchTerm = null,
    CancellationToken cancellationToken = default);
```

```csharp
// Example
var response = await client.GetPartsAsync(page: 1, pageSize: 50, includeDetails: true, searchTerm: "M-Tron");
var parts = response.Results;
```

#### Find part by BrickLink ID

```csharp
// API
Task<Part?> FindPartByBricklinkIdAsync(string bricklinkId,
    bool includeDetails = false,
    CancellationToken cancellationToken = default);
```

```csharp
// Example
var part = await client.FindPartByBricklinkIdAsync("3005", true);
```

#### Get part colors
```csharp
// API
Task<PagedResponse<PartColor>> GetPartColorsAsync(string partNumber,
    RebrickableCredentials? credentials = null,
    CancellationToken cancellationToken = default);
```

```csharp
// Example
var pagedResult = await client.GetPartColorDetailsAsync("3003");
var partColors = pagedResult.Results; 
```

#### Get part color details

```csharp
// API
Task<PartColorDetails> GetPartColorDetailsAsync(string partNumber, int colorId,
    CancellationToken cancellationToken = default);
```

```csharp
// Example
var colorId = 1; //Blue
var partColorDetails = await client.GetPartColorDetailsAsync("3005", colorId)
```

### Colors

#### Get colors

```csharp
// API
Task<PagedResponse<Color>> GetColorsAsync(int page = 1, int pageSize = 100,
    bool includeDetails = false,
    CancellationToken cancellationToken = default);
```

```csharp
// Example
var response = await client.GetColorsAsync(includeDetails: true,
    page: 1, pageSize: 50);
var colors = response.Results;
```

#### Get color

```csharp
// API
Task<Color> GetColorAsync(int colorId, bool includeDetails = false, 
    CancellationToken cancellationToken = default);
```

```csharp
// Example
var colorId = 0; //black
var black = await client.GetColorAsync(colorId, includeDetails: true);
```

### Elements

#### Get element

```csharp
// API
Task<Element> GetElementAsync(string elementId, 
    CancellationToken cancellationToken = default);  
```

```csharp
// Example
var elementId = "300521"; //1x1 Brick in Red
var element = await client.GetElementAsync(elementId);
```

### Minifigs

```csharp
// API
Task<Minifig> GetMinifigByIdAsync(
    string minifigId,
    RebrickableCredentials? credentials = null,
    CancellationToken cancellationToken = default);
```

```csharp
// Example
var minifigId = "fig-000001"; //Toy Store Employee, note that minifigId is named 'set_num' in the API docs.
var minifig = await client.GetMinifigByIdAsync(minifigId);
```