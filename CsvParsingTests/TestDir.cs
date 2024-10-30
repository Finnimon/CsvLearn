using Csv;

namespace CsvTests;

internal class TestDir
{
    #region Properties

    public (FileInfo file, Csv.Format format) WithHeader { get; }
    public (FileInfo file, Csv.Format format) WithOutHeader { get; }
    public (FileInfo file, Csv.Format format) BigWithHeader { get; }
    public (FileInfo file, Csv.Format format) BigWithoutHeader { get; }

    #endregion

    #region Construction

    public TestDir()
    {
        Csv.Format headerFormat=new(hasHeader:true);
        var headerLessFormat = new Csv.Format(hasHeader:false);

        WithHeader = (new("TestData\\WithHeader.csv"),headerFormat);
        WithOutHeader = (new("TestData\\WithOutHeader.csv"), headerLessFormat);
        BigWithHeader = (new("TestData\\BigWithHeader.csv"), headerFormat);
        BigWithoutHeader = (new("TestData\\BigWithoutHeader.csv"), headerLessFormat); 
    }

    #endregion
}