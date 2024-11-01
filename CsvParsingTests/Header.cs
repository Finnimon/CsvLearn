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
        var columns = reader.ReadColumns();

        var benchy = Factory.CreateBenchmarkReader(file, format);
        var benchyColumns = benchy.ReadColumns();

        foreach (var key in benchyColumns.Keys)
        {
            var column = columns[key];
            var benchyColumn = benchyColumns[key];

            for (var i = 0; i < benchyColumn.Count; i++) Assert.AreEqual(column[i], benchyColumn[i]);
        }
    }
}