using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using SpatialLite.Core.Geometries;
using SpatialLite.Core.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Benchmark.SpatialLite.Osm {
    public class Program {
        static void Main(string[] args) {
            var summary = BenchmarkRunner.Run<WktReaderBenchmarks>();
            //var test = new WktReaderBenchmarks();
            //test.ReadMemory();
        }
    }

    [MemoryDiagnoser]
    [SimpleJob(RunStrategy.Monitoring, targetCount: 50)]
    public class WktReaderBenchmarks {
        private byte[] _file;

        public WktReaderBenchmarks() {
            _file = File.ReadAllBytes("c:\\Users\\Lukas\\Source\\Repos\\spatiallite-net\\src\\Benchmark.SpatialLite.Osm\\TestFiles\\andorra-trim.wkt");
        }

        [Benchmark]
        public void ReadMemory() {
            var entitiesCount = 0;

            using (var reader = new WktReader(new MemoryStream(_file))) {
                Geometry entity;
                while ((entity = reader.Read()) != null) {
                    entitiesCount++;
                }
            }
        }

        [Benchmark]
        public void ReadFile() {
            var entitiesCount = 0;
            using (var reader = new WktReader("c:\\Users\\Lukas\\Source\\Repos\\spatiallite-net\\src\\Benchmark.SpatialLite.Osm\\TestFiles\\andorra-trim.wkt")) {
                Geometry entity;
                while ((entity = reader.Read()) != null) {
                    entitiesCount++;
                }
            }
        }
    }
}
