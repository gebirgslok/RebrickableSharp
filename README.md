# RebrickableSharp

## Introduction
RebrickableSharp is a strongly-typed, easy-to-use C# client for the [Rebrickable API](https://rebrickable.com/api/v3/docs/) that 
gets you started with just a few lines of code. It handles authentication, error handling and parsing of JSON into typed instances.
It supports all .NET platforms compatible with *.NET standard 2.0*.

## Related projects
- [BricklinkSharp](https://github.com/gebirgslok/BricklinkSharp) - Easy-to-use C# client for the bricklink (LEGO) marketplace API.

## Changelog

### 0.1.1
- Authentcation handling
- HTTP request / response handling
- New method on *IRebrickableClient*: **GetColorsAsync**. Returns a color list.
- New method on *IRebrickableClient*: **GetColorAsync**. Returns a specific color.
- New method on *IRebrickableClient*: **GetPartsAsync**. Returns a part list with various filter options.
- New method on *IRebrickableClient*: **GetElementAsync**. Returns a specific element.

## Get Started

### Demo Project
Check out the [demo project]https://github.com/gebirgslok/RebrickableSharp/tree/main/RebrickableSharp.Demos) for full-featured examples.

### Prerequisites
You need to have an account on [Rebrickable](https://www.rebrickable.com/). Then, go to *Account > Settings > API* and create a new **Key** or use an existing one.

### Install NuGet Package
#### Package Manager Console
 ```
Install-Package RebrickableSharp
```
#### Command Line
```
nuget install RebrickableSharp
```
