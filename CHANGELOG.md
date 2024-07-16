# Changelog

### 0.7.0
- Updated System.Text.Json NuGet dependency to v8.0.4 for .netstandard targets, see https://github.com/advisories/GHSA-hh2w-p6rv-4g7w
- New method on *IRebrickableClient*: **GetPartColorsAsync**, thanks to [stephanstapel](https://github.com/stephanstapel)

### 0.6.0
- New method on *IRebrickableClient*: **GetMinifigByIdAsync**. Returns a minifig by its ID.

### 0.5.0
- Added a new separate project and Nuget package for a **RebrickableCsvLoader** loader class which allows downloading and parsing CSV bulk files directly from the Rebrickable CDN, see the demo project for usage.
- New method on *IRebrickableClient*: **GetThemesAsync**. Returns all themes list.
- Moved the changelog to its own CHANGELOG.md file.

### 0.4.0
- Added optional parameter RebrickableCredentials? credentials = null to every method on IRebrickableClient that requires authentication. This will take precedence over the global credentials (RebrickableClientConfiguration.Instance) and is useful for server scenarios (e.g. ASP.NET Core) where you need to do a request in the name of the logged in user
- Throw exception if no API key is provided (neither global nor local)

### 0.3.0
- Added the option to inject a custom `IRebrickableRequestHandler` to handle the Rebrickable API rate limiting in your own way, thanks to [stephanstapel](https://github.com/stephanstapel)

### 0.2.1
- Fixed bug in **GetColorsAsync**: query params are not applied, thanks to [stephanstapel](https://github.com/stephanstapel)

### 0.2.0
- New method on *IRebrickableClient*: **GetMinifigsAsync**. Returns a list of Minifigs.
- New method on *IRebrickableClient*: **GetSetsAsync**. Returns a list of Sets.
- New method on *IRebrickableClient*: **GetSetPartsAsync**. Returns a list of all inventory parts in the specified Set.

Thanks to [stephanstapel](https://github.com/stephanstapel) for contributing the **GetSetsAsync** and **GetSetPartsAsync** methods.

### 0.1.0
- Authentication handling
- HTTP request / response handling
- New method on *IRebrickableClient*: **GetColorsAsync**. Returns a color list.
- New method on *IRebrickableClient*: **GetColorAsync**. Returns a specific color.
- New method on *IRebrickableClient*: **GetPartsAsync**. Returns a part list with various filter options.
- New method on *IRebrickableClient*: **FindPartByBricklinkIdAsync**. Find a part by its BrickLink ID.
- New method on *IRebrickableClient*: **GetPartColorDetails**. Gets details for a specific part / color combination.
- New method on *IRebrickableClient*: **GetElementAsync**. Returns a specific element.
