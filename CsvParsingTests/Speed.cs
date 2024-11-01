using Csv;
using System.Diagnostics;

namespace CsvTests;

[TestClass]
public class Speed
{
    [TestMethod]
    public void AbsoluteSpeedAcceptability()
    {
        Console.WriteLine("Make sure that you have compiler in Release Mode to ensure proper speed testing.");

        var (file, format) = new TestDir().BigWithHeader;
        var reader = Factory.CreateReader(file, format);

        var sW = Stopwatch.StartNew();
        _ = reader.Count();
        sW.Stop();

        var duration = sW.ElapsedMilliseconds;
        Console.WriteLine($"Duration in millis:\t{duration}");

        var benchy = Factory.CreateBenchmarkReader(file, format);

        sW.Restart();
        _ = benchy.Count();
        sW.Stop();

        Console.WriteLine($"Benchmark duration in millis:\t{sW.Elapsed.Milliseconds}");
        const int speedThreshold = 5;
        Assert.IsTrue(duration < sW.ElapsedMilliseconds * speedThreshold,
            $"Your implementation was more than {speedThreshold} times slower than my shitty benchy! Please optimize your dynamic parsing!");
    }
}