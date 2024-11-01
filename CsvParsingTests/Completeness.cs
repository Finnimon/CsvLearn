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
        var readerRecords = reader.ToList();

        var benchy = Factory.CreateBenchmarkReader(file, format);
        var benchyRecords = benchy.ToList();

        for (var i = 0; i < readerRecords.Count; i++)
            for (var j = 0; j < reader.ColumnCount; j++)
                Assert.AreEqual(benchyRecords[i][j], readerRecords[i][j]);
    }

    [TestMethod]
    public void WithoutHeader()
    {
        var (file, format) = new TestDir().WithHeader;
        var reader = Factory.CreateReader(file, format);
        var readerRecords = reader.ToList();

        var benchy = Factory.CreateBenchmarkReader(file, format);
        var benchyRecords = benchy.ToList();

        for (var i = 0; i < readerRecords.Count; i++)
            for (var j = 0; j < reader.ColumnCount; j++)
                Assert.AreEqual(benchyRecords[i][j], readerRecords[i][j]);
    }
}