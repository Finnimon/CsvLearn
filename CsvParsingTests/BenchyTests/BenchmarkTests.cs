using Csv;
using System.Diagnostics;

namespace CsvTests.BenchyTests;

[TestClass]
public class BenchmarkTests
{
    [TestMethod]
    public void TestCompleteness()
    {
        var (file, format) = new TestDir().WithHeader;
        const int expectedColumnCount = 12;
        const int expectedRowCount = 100;
        var benchy = Factory.CreateBenchmarkReader(file, format);
        var rowCount = 0;
        foreach (var record in benchy)
        {
            rowCount++;
            Assert.AreEqual(expectedColumnCount, record.Length);
        }

        Assert.AreEqual(expectedRowCount, rowCount);
    }

    [TestMethod]
    public void TestSpeed()
    {
        Console.WriteLine("Make sure that you have compiler in Release Mode to ensure proper speed testing.");

        var (file, format) = new TestDir().BigWithHeader;
        var benchy = Factory.CreateBenchmarkReader(file, format);

        var sw = Stopwatch.StartNew();
        var count = benchy.Count();
        sw.Stop();
        Console.WriteLine(
            $"Number of read records with {benchy.ColumnCount} fields:\t{count}\nRead in {sw.ElapsedMilliseconds} milliseconds.");
    }
}