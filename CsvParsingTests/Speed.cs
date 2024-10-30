using Csv;
using System.Diagnostics;
using System.Globalization;

namespace CsvTests;

[TestClass]
public class Speed
{
    [TestMethod]
    public void AbsoluteSpeedAcceptability()
    {
        var (file, format) = new TestDir().BigWithHeader;
        var reader = Csv.Factory.CreateReader(file, format);

        var sW = Stopwatch.StartNew();
        var count = reader.Count();
        sW.Stop();

        var duration = sW.Elapsed;
        Console.WriteLine($"Duration in millis:\t{duration.Milliseconds}");

        var benchy = Factory.CreateBenchmarkReader(file, format);
        
        sW.Restart();
        var benchyCount= benchy.Count();
        sW.Stop();

        Assert.AreEqual(benchyCount,count);

        Console.WriteLine($"Benchmark duration in millis:\t{sW.Elapsed.Milliseconds}");

        Assert.IsTrue(duration.Milliseconds < sW.ElapsedMilliseconds * 10,"Implementation was more than 10 times slower than my shitty benchy! Please optimize your dynamic parsing!");
    }
}