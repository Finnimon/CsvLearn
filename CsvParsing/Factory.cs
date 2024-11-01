using Csv.Benchmark;

namespace Csv;

public static class Factory
{
    /// <summary>
    ///     Creates a <see cref="IReader" /> with the given Arguments.
    /// </summary>
    /// <param name="file">The file location of the Csv.</param>
    /// <param name="hasHeader">Whether the csv has a header.</param>
    /// <param name="separator">The value separator used by the csv.</param>
    /// <param name="newLine">The newline delimiter used by the csv.</param>
    /// <returns>The created <see cref="IReader" />.</returns>
    /// <exception cref="FileNotFoundException">
    ///     Thrown when <paramref name="file" /> does not exist.
    /// </exception>
    /// <exception cref="FormatException">
    ///     Thrown when <paramref name="file" /> is not formatted correctly.
    /// </exception>
    public static IReader CreateReader(FileInfo file, bool hasHeader = false, char separator = ',',
        string newLine = "\r\n", char regexEscape = '"') => throw new NotImplementedException();

    /// <summary>
    ///     Creates a <see cref="IReader" /> with the given Arguments.
    /// </summary>
    /// <param name="file">The file location of the Csv.</param>
    /// <param name="format">The format used by <paramref name="file" />.</param>
    /// <returns>The created <see cref="IReader" />.</returns>
    /// <exception cref="FileNotFoundException">
    ///     Thrown when <paramref name="file" /> does not exist.
    /// </exception>
    /// <exception cref="FormatException">
    ///     Thrown when <paramref name="file" /> is not formatted correctly.
    /// </exception>
    public static IReader CreateReader(FileInfo file, Format format = new()) => throw new NotImplementedException();

    public static IReader CreateBenchmarkReader(FileInfo file, Format format = new()) => new Reader(file.OpenText(), format);
}