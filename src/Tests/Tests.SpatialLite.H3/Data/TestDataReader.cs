using System.Reflection;

namespace Tests.SpatialLite.H3.Data;

/// <summary>
/// Helper class for loading test data files.
/// </summary>
public class TestDataReader
{
    private readonly string _dataFolderPath;

    /// <summary>
    /// Initializes a new instance of the TestDataReader class.
    /// </summary>
    /// <param name="folderPath">Relative folder path for test data files</param>
    public TestDataReader(string folderPath)
    {
        var assembly = typeof(TestDataReader).GetTypeInfo().Assembly;

        // Build path to the output directory where files are copied
        var assemblyLocation = assembly.Location;
        var assemblyDirectory = Path.GetDirectoryName(assemblyLocation)
            ?? throw new InvalidOperationException("Could not determine assembly directory.");
        _dataFolderPath = Path.Combine(assemblyDirectory, "Data", folderPath);
    }

    /// <summary>
    /// Opens a stream to the test data file.
    /// </summary>
    /// <param name="name">Name of the test data file</param>
    /// <returns>Stream of the test data</returns>
    public Stream Open(string name)
    {
        var filePath = GetPath(name);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Test data file not found: {filePath}");
        }

        return File.OpenRead(filePath);
    }

    /// <summary>
    /// Reads the test data file into a byte array.
    /// </summary>
    /// <param name="name">Name of the test data file</param>
    /// <returns>Byte array containing the test data</returns>
    public byte[] Read(string name)
    {
        var filePath = GetPath(name);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Test data file not found: {filePath}");
        }

        return File.ReadAllBytes(filePath);
    }

    /// <summary>
    /// Gets the full path to a test data file.
    /// </summary>
    /// <param name="name">Name of the test data file</param>
    /// <returns>Full path to the test data file</returns>
    public string GetPath(string name)
    {
        return Path.Combine(_dataFolderPath, name);
    }

    /// <summary>
    /// TestDataReader for H3 compatibility test data.
    /// </summary>
    public static readonly TestDataReader H3 = new("H3");
}
