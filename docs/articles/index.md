# Getting started

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