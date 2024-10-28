using System.Globalization;
using System.Net.Mime;
using Csv;

namespace CsvTests;

[TestClass]
public class Completeness
{
    [TestMethod]
    public void WithHeader()
    {
        var (file, format) = new TestDir().WithHeader;
        var reader = Factory.CreateReader(file, format);
        reader.ReadCompletely();

        var csvHelperReader = new CsvHelper.CsvReader(file.OpenText(), CultureInfo.CurrentCulture, false);
        csvHelperReader.ReadHeader();

        var csvHelperRecords = csvHelperReader.GetRecords<string[]>().ToList();
        var readerRecords = reader.ToList();
        for (var i = 0; i < readerRecords.Count; i++)
        for (var j = 0; j < reader.ColumnCount; j++) Assert.AreEqual(csvHelperRecords[i][j], readerRecords[i][j]);
    }

    [TestMethod]
    public void WithoutHeader()
    {
        var (file, format) = new TestDir().WithOutHeader;
        var reader = Factory.CreateReader(file, format);
        reader.ReadCompletely();

        var csvHelperReader = new CsvHelper.CsvReader(file.OpenText(), CultureInfo.CurrentCulture, false);
        csvHelperReader.ReadHeader();
        var csvHelperRecords = csvHelperReader.GetRecords<string[]>().ToList();
        var readerRecords = reader.ToList();
        for (var i = 0; i < readerRecords.Count; i++)
        for (var j = 0; j < reader.ColumnCount; j++) Assert.AreEqual(csvHelperRecords[i][j], readerRecords[i][j]);
    }
}