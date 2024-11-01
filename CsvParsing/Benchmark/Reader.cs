using System.Collections;
using System.Text;

namespace Csv.Benchmark;

internal sealed class Reader : IReader
{
    #region Properties

    private bool Disposed { get; set; }
    private List<string[]> _records;
    public char RegexEscape { get; }
    public char Separator { get; }
    public string LineBreak { get; }
    private readonly char _lineBreakFirst;
    private bool _simpleLineBreakMode;
    public TextReader StringInput { get; }
    public int ColumnCount { get; private set; }
    public bool HasHeader { get; }
    public string[]? Header { get; }

    #endregion

    #region Construction

    public Reader(TextReader stringInput, Format format = new())
    {
        StringInput = stringInput;
        Disposed = false;
        RegexEscape = format.RegexEscape;
        Separator = format.Separator;
        LineBreak = format.LineBreak;
        _lineBreakFirst = LineBreak[0];
        _simpleLineBreakMode = LineBreak.Length == 1;
        HasHeader = format.HasHeader;
        Header = !HasHeader ? null : ReadRecord();
        ColumnCount = !HasHeader ? 0 : Header!.Length;
        _records = [];
    }

    public Reader(TextReader stringInput, char regexEscape = '"', char separator = ',', string lineBreak = "\r\n",
        bool hasHeader = false)
    {
        StringInput = stringInput;
        Disposed = false;
        RegexEscape = regexEscape;
        Separator = separator;
        LineBreak = lineBreak;
        _lineBreakFirst = LineBreak[0];
        _simpleLineBreakMode = LineBreak.Length == 1;
        HasHeader = hasHeader;
        Header = !HasHeader ? null : ReadRecord();
        ColumnCount = !HasHeader ? 0 : Header!.Length;
        _records = [];
    }

    #endregion

    #region IReader Implementation

    public IEnumerable<string> ReadColumn(string columnName) => ReadColumns([columnName])[columnName];

    public Dictionary<string, List<string>> ReadColumns(IEnumerable<string> columnNames)
    {
        if (!HasHeader) throw new InvalidOperationException("No Header");

        List<int> columnIndexes = [];
        var headerList = Header!.ToList();
        columnIndexes.AddRange(columnNames.Select(columnName => headerList.IndexOf(columnName)).Where(x => x != -1));
        Dictionary<string, List<string>> columns = [];
        foreach (var index in columnIndexes) columns[Header![index]] = [];
        foreach (var record in this)
            foreach (var columnIndex in columnIndexes)
                columns[Header![columnIndex]].Add(record[columnIndex]);
        return columns;
    }

    public Dictionary<string, List<string>> ReadColumns() => !HasHeader ? throw new InvalidOperationException("No Header") : ReadColumns(Header!);

    #endregion

    #region IEnumerable Implementation

    private class Enumerator : IEnumerator<string[]>
    {
        private int _index;
        private readonly Reader _reader;
        private string[]? _current;
        public string[] Current
        {
            get
            {
                _ = _current ?? throw new InvalidOperationException();
                return _current;
            }
            private set => _current = value;
        }

        object IEnumerator.Current => Current;


        public Enumerator(Reader reader)
        {
            if (reader.Disposed)
                throw new ObjectDisposedException($"{nameof(reader)} of type {typeof(Reader)} is disposed.");
            _index = -1;
            _reader = reader;
            _current = null;
        }
        public bool MoveNext()
        {
            _index++;

            if (_reader._records.Count > _index)
            {
                Current = _reader._records[_index];
                return true;
            }

            return TryReadUntilIndex();
        }

        private bool TryReadUntilIndex()
        {
            try
            {
                while (_index >= _reader._records.Count)
                {
                    Current = _reader.ReadRecord();
                    _reader._records.Add(Current);
                }
            }
            catch (EndOfStreamException)
            {
                return false;
            }

            return true;
        }

        public void Reset() => throw new InvalidOperationException();


        public void Dispose() => _reader.Dispose();
    }

    public IEnumerator<string[]> GetEnumerator() => Disposed ? _records.GetEnumerator() : new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Dispose()
    {
        Disposed = true;
        StringInput.Dispose();
    }

    #endregion

    #region Private Helper Methods

    private string[] ReadRecord()
    {
        if (Disposed) throw new EndOfStreamException();
        var record = new List<string>(ColumnCount);
        var stringBuilder = new StringBuilder();
        var readResult = ReadResults.Unknown;

        while (readResult is not (ReadResults.EndOfRecord or ReadResults.EndOfStream))
        {
            readResult = ReadField(stringBuilder);
            record.Add(stringBuilder.ToString());
            _ = stringBuilder.Clear();
        }

        var isEndOfStream = readResult is ReadResults.EndOfStream;
        if (isEndOfStream) Dispose();
        if (isEndOfStream && record.Count == 1 && !record.First().Any())
            throw new EndOfStreamException();
        if (!HasHeader) ColumnCount = record.Count;
        return record.ToArray();
    }

    private ReadResults ReadField(StringBuilder stringBuilder)
    {
        var regexEscaped = false;
        var readResult = ReadResults.Incomplete;
        while (readResult == ReadResults.Incomplete) readResult = ReadNextChar(stringBuilder, ref regexEscaped);
        return readResult;
    }

    private ReadResults ReadNextChar(StringBuilder stringBuilder, ref bool regexEscaped)
    {
        var readInt = StringInput.Read();
        if (readInt is -1) return ReadResults.EndOfStream;
        var read = (char)readInt;

        if (read == Separator)
            return HandleSeparator(stringBuilder, regexEscaped, read);
        if (read == _lineBreakFirst)
            return HandlePossibleLineBreak(stringBuilder, regexEscaped);
        if (read == RegexEscape)
        {
            regexEscaped = HandleEscape(stringBuilder, regexEscaped, read);
            return ReadResults.Incomplete;
        }

        _ = stringBuilder.Append(read);
        return ReadResults.Incomplete;
    }

    private static ReadResults HandleSeparator(StringBuilder stringBuilder, bool regexEscaped, char read)
    {
        if (!regexEscaped) return ReadResults.Success;
        _ = stringBuilder.Append(read);
        return ReadResults.Incomplete;
    }

    private ReadResults HandlePossibleLineBreak(StringBuilder stringBuilder, bool regexEscaped)
    {
        var lineBreakFound = CheckLineBreak(regexEscaped, stringBuilder);
        return !lineBreakFound || regexEscaped ? ReadResults.Incomplete : ReadResults.EndOfRecord;
    }

    private bool HandleEscape(StringBuilder stringBuilder, bool regexEscaped, char read)
    {
        if (!regexEscaped) return true;

        if ((char)StringInput.Peek() != RegexEscape) return false;

        _ = StringInput.Read();
        _ = stringBuilder.Append(read);
        return regexEscaped;
    }

    private bool CheckLineBreak(bool regexEscaped, StringBuilder stringBuilder)
    {
        var checkedChars = new StringBuilder(LineBreak.Length);
        _ = checkedChars.Append(LineBreak[0]);
        var lineBreakFound = CheckForString(StringInput, LineBreak, 1, LineBreak.Length, checkedChars);

        if (regexEscaped) _ = stringBuilder.Append(checkedChars);

        return lineBreakFound;
    }

    private static bool CheckForString(TextReader stringInput, string check, int checkIndex, int checkLength,
        StringBuilder readChars)
    {
        checkLength += checkIndex;
        for (; checkIndex < checkLength && checkIndex < check.Length; checkIndex++)
        {
            var read = (char)stringInput.Peek();
            if (read != check[checkIndex]) return false;
            _ = stringInput.Read();
            _ = readChars.Append(read);
        }

        return true;
    }

    private RegexType GetRegexType(char read) => read == Separator
            ? RegexType.Separator
            : read == RegexEscape ? RegexType.Escape : read == _lineBreakFirst ? RegexType.LineBreakFirst : RegexType.None;

    #endregion

    #region private HelperType

    private enum RegexType
    {
        None,
        Escape,
        Separator,
        LineBreakFirst
    }

    private enum ReadResults
    {
        Incomplete,
        Success,
        EndOfStream,
        EndOfRecord,
        Unknown
    }

    #endregion
}