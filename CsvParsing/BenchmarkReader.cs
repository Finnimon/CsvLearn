using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Csv;

internal class BenchmarkReader : IReader
{
    #region Properties

    private bool Disposed { get; set; }
    public char RegexEscape { get; }
    public char Separator { get; }
    public string LineBreak { get; }
    public TextReader StringInput { get; }
    public int ColumnCount { get; }
    public bool HasHeader { get; }
    public string[]? Header { get; }
    private char[] RegexChars { get; }


    #endregion

    #region Construction

    
    public BenchmarkReader(TextReader stringInput, Format format = new Format())
    {
        StringInput = stringInput;
        Disposed = false;
        RegexEscape = format.RegexEscape;
        Separator = format.Separator;
        LineBreak = format.LineBreak;
        HasHeader = format.HasHeader;
        Header = !HasHeader ? null : ReadRecord();
        ColumnCount = !HasHeader ? -1 : Header!.Length;
        RegexChars = [RegexEscape, Separator];
    }

    public BenchmarkReader(TextReader stringInput, char regexEscape = '"', char separator = ',', string lineBreak = "\r\n", bool hasHeader = false)
    {
        Disposed = false;
        RegexEscape = regexEscape;
        Separator = separator;
        LineBreak = lineBreak;
        StringInput = stringInput;
        HasHeader = hasHeader;
        Header = !hasHeader ? null : ReadRecord();
        ColumnCount = !hasHeader ? -1 : Header!.Length;
        RegexChars = [regexEscape, separator];
    }

    #endregion

    #region IReader Implementation




    public IEnumerable<string> ReadColumn(string columnName)
    {
        return ReadColumns([columnName])[columnName];
    }

    public Dictionary<string, List<string>> ReadColumns(IEnumerable<string> columnNames)
    {
        if (!HasHeader) throw new InvalidOperationException("No Header");

        List<int> columnIndexes = [];
        var headerList = Header!.ToList();
        columnIndexes.AddRange(columnNames.Select(columnName => headerList.IndexOf(columnName)).Where(x => x != -1));
        Dictionary<string, List<string>> columns = [];
        foreach (var index in columnIndexes) columns[Header![index]] = [];
        foreach (var record in this)
            foreach (var columnIndex in columnIndexes) columns[Header[columnIndex]].Add(record[columnIndex]);
        return columns;
    }

    public Dictionary<string, List<string>> ReadColumns()
    {
        if (!HasHeader) throw new InvalidOperationException("No Header");

        return ReadColumns(Header!);
    }

    #endregion
    #region IEnumerable Implementation

    private class Enumerator : IEnumerator<string[]>
    {
        private BenchmarkReader _benchmarkReader;
        public string[]? Current { get; private set; }
        object? IEnumerator.Current => Current;

        public Enumerator(BenchmarkReader benchmarkReader)
        {
            if (benchmarkReader.Disposed) throw new ObjectDisposedException($"{nameof(benchmarkReader)} of type {typeof(BenchmarkReader)} is disposed.");
            _benchmarkReader = benchmarkReader;
            Current = null;
        }

        public bool MoveNext()
        {
            try
            {
                Current = _benchmarkReader.ReadRecord();
            }
            catch (EndOfStreamException e)
            {
                return false;
            }
            return true;
        }

        public void Reset() => throw new InvalidOperationException();


        public void Dispose() { }

    }

    public IEnumerator<string[]> GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Dispose()
    {
        Disposed = true;
        StringInput.Dispose();
    }

    #endregion

    #region Private Helper Methods

    private string[]? ReadRecord()
    {
        if (Disposed) throw new EndOfStreamException();
        List<string> record = [];
        var stringBuilder = new StringBuilder();
        var readResult = ReadResults.Unknown;

        while (readResult is not (ReadResults.EndOfRecord or ReadResults.EndOfStream))
        {
            readResult = ReadField(stringBuilder);
            record.Add(stringBuilder.ToString());
            stringBuilder.Clear();
        }

        if (readResult is ReadResults.EndOfStream) Dispose();

        return record.ToArray();
    }

    private ReadResults ReadField(StringBuilder stringBuilder)
    {
        var fieldComplete = false;
        var regexEscaped = false;
        var escapedFound = false;
        while (!fieldComplete)
        {

            var readInt = StringInput.Read();
            if (readInt is -1) return ReadResults.EndOfStream;
            var read = (char)readInt;
            if (LineBreak[0] == read)
            {
                var lineBreakFound = CheckLineBreak(regexEscaped, stringBuilder);
                if (regexEscaped) escapedFound = lineBreakFound;
                if (lineBreakFound && regexEscaped) continue;
                return StringInput.Peek() is -1 ? ReadResults.EndOfStream : ReadResults.EndOfRecord;
            }


            if (read == RegexEscape && (!regexEscaped || escapedFound))
            {
                regexEscaped = !regexEscaped;
                continue;
            }

            if (regexEscaped && RegexChars.Contains(read)) escapedFound = true;


            if (!regexEscaped && read == Separator)
            {
                fieldComplete = true;
                continue;
            }


            stringBuilder.Append(read);
        }

        return ReadResults.Success;
    }

    private bool CheckLineBreak(bool regexEscaped, StringBuilder stringBuilder)
    {
        var checkedChars = new StringBuilder(LineBreak.Length);
        checkedChars.Append(LineBreak[0]);
        var lineBreakFound = CheckForString(StringInput, LineBreak, 1, LineBreak.Length, checkedChars);

        if (regexEscaped) stringBuilder.Append(checkedChars);

        return lineBreakFound;
    }

    private static bool CheckForString(TextReader stringInput, string check, int checkIndex, int checkLength, StringBuilder readChars)
    {
        checkLength += checkIndex;
        for (; checkIndex < checkLength && checkIndex < check.Length; checkIndex++)
        {
            var read = (char)stringInput.Peek();
            if (read != check[checkIndex]) return false;
            stringInput.Read();
            readChars.Append(read);
        }
        return true;
    }

    #endregion

    #region private HelperType

    private enum ReadResults
    {
        Success,
        EndOfStream,
        EndOfRecord,
        Unknown
    }

    #endregion
}