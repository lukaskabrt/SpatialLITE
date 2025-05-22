using SpatialLite.Osm;
using SpatialLite.Osm.Geometries;
using SpatialLite.Osm.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Benchmark.SpatialLite.Osm;

public class Program
{
    private static List<IEntityInfo> _entities;

    private static void Main(string[] args)
    {
        List<Tuple<string, Action>> benchmarks = new List<Tuple<string, Action>>();
        benchmarks.Add(new Tuple<string, Action>("XmlReader with metadata", TestXmlReaderSpeed));
        benchmarks.Add(new Tuple<string, Action>("XmlReader without metadata", TestXmlReaderSpeedWithoutMetadata));
        //benchmarks.Add(new Tuple<string, Action>("PbfReader (no compression) with metadata", TestPbfReaderSpeedNoDenseNoCompression));
        //benchmarks.Add(new Tuple<string, Action>("PbfReader (no compression) without metadata", TestPbfReaderSpeedNoDenseNoCompressionWithoutMetadata));
        //benchmarks.Add(new Tuple<string, Action>("PbfReader (dense, deflate) with metadata", TestPbfReaderSpeedDenseDeflate));
        //benchmarks.Add(new Tuple<string, Action>("PbfReader (dense, deflate) without metadata", TestPbfReaderSpeedDenseDeflateWithoutMetadata));

        //LoadSourceData();
        //benchmarks.Add(new Tuple<string, Action>("XmlWriter with metadata", TestXmlWriterSpeed));
        //benchmarks.Add(new Tuple<string, Action>("XmlWriter without metadata", TestXmlWriterSpeedWithoutMetadata));
        //benchmarks.Add(new Tuple<string, Action>("PbfWriter (no compression) with metadata", TestPbfWriterSpeed));
        //benchmarks.Add(new Tuple<string, Action>("PbfWriter (no compression) without metadata", TestPbfWriterSpeedWithoutMetadata));
        //benchmarks.Add(new Tuple<string, Action>("PbfWritre (dense, deflate) with metadata", TestPbfWriterSpeedDenseDeflate));
        //benchmarks.Add(new Tuple<string, Action>("PbfWriter (dense, deflate) without metadata", TestPbfWriterSpeedDenseDeflateWithoutMetadata));

        //benchmarks.Add(new Tuple<string, Action>("OsmGeometryDatabase.Load(PbfReader)", TestOsmGeometryDatabaseLoadFromPbfReader));
        //benchmarks.Add(new Tuple<string, Action>("OsmEntityInfoDatabase.Load(PbfReader)", TestOsmEntityInfoDatabaseLoadFromPbfReader));

        Console.WriteLine("Benchmark.SpatialLite.Osm.IO requires 'test-file.osm', 'test-file.pbf' and 'test-file-dc.pbf' in 'TestFiles'folder");

        using (var stream = new FileStream("Benchmark.SpatialLite.Osm.IO.log", FileMode.Create, FileAccess.Write))
        {
            using (TextWriter tw = new StreamWriter(stream))
            {
                foreach (var benchmark in benchmarks)
                {
                    long avgTime = avgTime = DoTest(benchmark.Item2, benchmark.Item1);
                    tw.WriteLine(string.Format("{0} ms \t\t{1}", avgTime, benchmark.Item1));
                }
            }
        }
    }

    private static long DoTest(Action testAction, string testName)
    {
        Console.WriteLine(string.Format("Starting benchmark '{0}'", testName));

        long totalTime = 0;
        for (int i = 0; i < 10; i++)
        {
            Console.Write(string.Format("Run ({0}/{1}) ...", i + 1, 10));
            Stopwatch watch = new Stopwatch();
            watch.Start();

            testAction();

            watch.Stop();
            totalTime += watch.ElapsedMilliseconds;
            Console.WriteLine(string.Format("\t\t({0} ms)", watch.ElapsedMilliseconds));
        }

        Console.WriteLine();
        Console.WriteLine("Average runnig time: {0} ms\n", totalTime / 10);
        return totalTime / 10;
    }

    private static void LoadSourceData()
    {
        _entities = new List<IEntityInfo>();

        IEntityInfo info = null;
        using (PbfReader reader = new PbfReader("TestFiles\\test-file-dc.pbf", new OsmReaderSettings() { ReadMetadata = true }))
        {
            while ((info = reader.Read()) != null)
            {
                _entities.Add(info);
            }
        }
    }

    private static void TestXmlReaderSpeed()
    {
        int entitiesRead = 0;
        using (OsmXmlReader reader = new OsmXmlReader("TestFiles\\test-file.osm", new OsmXmlReaderSettings() { ReadMetadata = true }))
        {

            IEntityInfo info;
            while ((info = reader.Read()) != null)
            {
                entitiesRead++;
            }
        }
    }

    private static void TestXmlReaderSpeedWithoutMetadata()
    {
        int entitiesRead = 0;
        using (OsmXmlReader reader = new OsmXmlReader("TestFiles\\test-file.osm", new OsmXmlReaderSettings() { ReadMetadata = false }))
        {

            IEntityInfo info;
            while ((info = reader.Read()) != null)
            {
                entitiesRead++;
            }
        }
    }

    private static void TestXmlWriterSpeed()
    {
        using (OsmXmlWriter writer = new OsmXmlWriter("TestFiles\\temp.osm", new OsmWriterSettings() { WriteMetadata = true }))
        {
            foreach (var entity in _entities)
            {
                writer.Write(entity);
            }
        }
    }

    private static void TestXmlWriterSpeedWithoutMetadata()
    {
        using (OsmXmlWriter writer = new OsmXmlWriter("TestFiles\\temp.osm", new OsmWriterSettings() { WriteMetadata = false }))
        {
            foreach (var entity in _entities)
            {
                writer.Write(entity);
            }
        }
    }

    private static void TestPbfReaderSpeedNoDenseNoCompression()
    {
        int entitiesRead = 0;
        using (PbfReader reader = new PbfReader("TestFiles\\test-file.pbf", new OsmReaderSettings() { ReadMetadata = true }))
        {

            IEntityInfo info;
            while ((info = reader.Read()) != null)
            {
                entitiesRead++;
            }
        }
    }

    private static void TestPbfReaderSpeedNoDenseNoCompressionWithoutMetadata()
    {
        int entitiesRead = 0;
        using (PbfReader reader = new PbfReader("TestFiles\\test-file.pbf", new OsmReaderSettings() { ReadMetadata = false }))
        {

            IEntityInfo info;
            while ((info = reader.Read()) != null)
            {
                entitiesRead++;
            }
        }
    }

    private static void TestPbfReaderSpeedDenseDeflate()
    {
        int entitiesRead = 0;
        using (PbfReader reader = new PbfReader("TestFiles\\test-file-dc.pbf", new OsmReaderSettings() { ReadMetadata = true }))
        {

            IEntityInfo info;
            while ((info = reader.Read()) != null)
            {
                entitiesRead++;
            }
        }
    }

    private static void TestPbfReaderSpeedDenseDeflateWithoutMetadata()
    {
        int entitiesRead = 0;
        using (PbfReader reader = new PbfReader("TestFiles\\test-file-dc.pbf", new OsmReaderSettings() { ReadMetadata = false }))
        {

            IEntityInfo info;
            while ((info = reader.Read()) != null)
            {
                entitiesRead++;
            }
        }
    }

    private static void TestPbfWriterSpeed()
    {
        using (PbfWriter writer = new PbfWriter("TestFiles\\temp.pbf", new PbfWriterSettings() { WriteMetadata = true, Compression = CompressionMode.None, UseDenseFormat = false }))
        {
            foreach (var entity in _entities)
            {
                writer.Write(entity);
            }
        }
    }

    private static void TestPbfWriterSpeedWithoutMetadata()
    {
        using (PbfWriter writer = new PbfWriter("TestFiles\\temp.pbf", new PbfWriterSettings() { WriteMetadata = false, Compression = CompressionMode.None, UseDenseFormat = false }))
        {
            foreach (var entity in _entities)
            {
                writer.Write(entity);
            }
        }
    }

    private static void TestPbfWriterSpeedDenseDeflate()
    {
        using (PbfWriter writer = new PbfWriter("TestFiles\\temp.pbf", new PbfWriterSettings() { WriteMetadata = true, Compression = CompressionMode.ZlibDeflate, UseDenseFormat = true }))
        {
            foreach (var entity in _entities)
            {
                writer.Write(entity);
            }
        }
    }

    private static void TestPbfWriterSpeedDenseDeflateWithoutMetadata()
    {
        using (PbfWriter writer = new PbfWriter("TestFiles\\temp.pbf", new PbfWriterSettings() { WriteMetadata = false, Compression = CompressionMode.ZlibDeflate, UseDenseFormat = true }))
        {
            foreach (var entity in _entities)
            {
                writer.Write(entity);
            }
        }
    }

    private static void TestOsmGeometryDatabaseLoadFromPbfReader()
    {
        using (PbfReader reader = new PbfReader("TestFiles\\test-file-dc.pbf", new OsmReaderSettings() { ReadMetadata = true }))
        {
            OsmGeometryDatabase db = OsmGeometryDatabase.Load(reader, true);
        }
    }

    private static void TestOsmEntityInfoDatabaseLoadFromPbfReader()
    {
        using (PbfReader reader = new PbfReader("TestFiles\\test-file-dc.pbf", new OsmReaderSettings() { ReadMetadata = true }))
        {
            OsmEntityInfoDatabase db = OsmEntityInfoDatabase.Load(reader);
        }
    }
}
