namespace Csv;

/// <summary>
/// <para>Object for parsing CsvFile.</para>
/// <para>Should parse line by line dynamically as it is enumerated.</para>
/// <para>Enumeration should skip the header line, if one exists.</para>
/// </summary>
public interface IReader : IEnumerable<string[]>, IDisposable
{
    #region Properties
    /// <summary>
    /// The delimiter separating values.
    /// </summary>
    public char Separator { get; }

    /// <summary>
    /// The delimiter string for new lines.
    /// </summary>
    public string LineBreak { get; }

    /// <summary>
    /// The File location of the Csv.
    /// </summary>
    protected TextReader StringInput { get; }

    public int ColumnCount { get; }

    /// <summary>
    /// Whether the Csv has a header.
    /// </summary>
    public bool HasHeader { get; }

    /// <summary>
    /// CsvHeader or null if there is none.
    /// </summary>
    public string[]? Header { get; }
    #endregion

    #region Methods


    /// <summary>
    /// Read the column denominated by <paramref name="columnName"/>.
    /// </summary>
    /// <exception cref="NotSupportedException">
    /// Thrown when <see cref="HasHeader"/><code>=false</code>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="columnName"/> does not exist.
    /// </exception>
    public IEnumerable<string> ReadColumn(string columnName);

    /// <summary>
    /// Read all columns denominated by <paramref name="columnNames"/>.
    /// </summary>
    /// <param name="columnNames"></param>
    /// <exception cref="NotSupportedException">
    /// Thrown when <see cref="HasHeader"/><code>=false</code>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when any of the <paramref name="columnNames"/> do not exist.
    /// </exception>
    public Dictionary<string, List<string>> ReadColumns(IEnumerable<string> columnNames);

    /// <summary>
    /// Read All Columns in the File.
    /// </summary>
    /// <exception cref="NotSupportedException">
    /// Thrown when <see cref="HasHeader"/><code>=false</code>.
    /// </exception>
    public Dictionary<string, List<string>> ReadColumns();


    #endregion
}