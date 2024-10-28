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
        foreach (var record in reader) Console.WriteLine(record);
        sW.Stop();
        reader.Dispose();
        var duration = sW.Elapsed;
        Console.WriteLine($"Duration in millis:\t{duration.Milliseconds}");
        
        var csvHelperReader = new CsvHelper.CsvReader(file.OpenText(), CultureInfo.CurrentCulture, false);
        var recordCsvHelper = new string[csvHelperReader.ColumnCount];
        var csvHelperRecords= csvHelperReader.EnumerateRecords(recordCsvHelper);
        sW.Restart();
        foreach (var csvHelperRecord in csvHelperRecords) Console.WriteLine(csvHelperRecord);
        sW.Stop();

        Console.WriteLine($"Benchmark duration in millis:\t{sW.Elapsed.Milliseconds}");

        Assert.IsTrue(duration.Milliseconds < sW.ElapsedMilliseconds * 1000,"Implementation was more than 1000 times slower than benchmark! Please optimize your dynamic parsing!");
    }
}