using System.Diagnostics;
using System.Globalization;
using Csv;

namespace CsvTests;

[TestClass]
public class Header
{
    [TestMethod]
    public void Queries()
    {
        var (file, format) = new TestDir().WithHeader;
        var reader = Factory.CreateReader(file, format);
        reader.ReadCompletely();

        var csvHelperReader = new CsvHelper.CsvReader(file.OpenText(), CultureInfo.CurrentCulture, false);
        csvHelperReader.ReadHeader();

        Assert.AreEqual(csvHelperReader.ColumnCount, reader.ColumnCount);

        for (var i = 0; i < csvHelperReader.HeaderRecord.Length; i++) Assert.AreEqual(csvHelperReader.HeaderRecord[i], reader.Header[i]);

    }

}