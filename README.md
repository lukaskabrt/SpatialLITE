# SpatialLITE

SpatialLITE is a lightweight .NET Core library for spatial data processing.

![Build Status](https://kabrt.visualstudio.com/_apis/public/build/definitions/a0c2814c-9acd-4035-824a-a2548f8a8c1b/10/badge)


## Features
* Reads / writes data in variety of formats
    * WKT (Well-known text)
    * WKB (Well-known binary)
    * GPX
    * OpenStreetMap XML
    * OpenStreetMap PBF
* Analytics functions
    * Distance / area calculations
    * Basic topology analysis


## Getting started

Install the NuGet package:
``` ps
Install-Package SpatialLite.Core
```

Use library:
``` c#
var geometry = WktReader.Parse<LineString>("linestring (-10.1 15.5, 20.2 -25.5, 30.3 35.5)");

Console.WriteLine("Start point: {0}", geometry.Start);
Console.WriteLine("End point: {0}", geometry.End);

// Writes:
// Start point: [-10.1; 15.5; NaN, NaN]
// End point: [30.3; 35.5; NaN, NaN]
```

## Packages

SpatialLITE is distributed as a set of NuGet packages. You can install only packages, you need.

## Documentation

API reference and more complex examples are available at http://spatial.litesolutions.net


### SpatialLITE.Core [![NuGet](https://img.shields.io/nuget/v/SpatialLite.Core.svg)](https://www.nuget.org/packages/SpatialLite.Core) [![Package stats SpatialLITE](https://img.shields.io/nuget/dt/SpatialLITE.Core.svg)](https://www.nuget.org/packages/SpatialLite.Core)

Provides contains basic data structures for geospatial data and common infrastructure. It is required by all provided add-ons.


### SpatialLITE.Gps [![NuGet](https://img.shields.io/nuget/v/SpatialLite.Gps.svg)](https://www.nuget.org/packages/SpatialLite.Gps) [![Package stats SpatialLITE.Gps](https://img.shields.io/nuget/dt/SpatialLITE.Gps.svg)](https://www.nuget.org/packages/SpatialLite.Gps)

Add-on for the SpatialLite library that adds support for GPX data format.


### SpatialLITE.Osm [![NuGet](https://img.shields.io/nuget/v/SpatialLite.Osm.svg)](https://www.nuget.org/packages/SpatialLite.Osm) [![Package stats SpatialLITE.Osm](https://img.shields.io/nuget/dt/SpatialLITE.Osm.svg)](https://www.nuget.org/packages/SpatialLite.Osm)

Add-on for the SpatialLite library that adds support for OpenStreetMap data formats.


## Community
* Have you find a bug? Do you have an idea for a new feature? ... [open an issue on GitHub](https://github.com/lukaskabrt/spatiallite-net/issues)
* Do you have a question? ... [ask on StackOverflow](https://stackoverflow.com/questions/ask?tags=spatiallite)
* Do you want to contribute piece of code? ... [submit a pull-request](https://github.com/lukaskabrt/spatiallite-net/pulls)
    * `master` branch contains the code being worked on


## License [![MIT License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details