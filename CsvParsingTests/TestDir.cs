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
        WithHeader = (new("TestData\\WithHeader.csv"),new Csv.Format());
        WithOutHeader = (new("TestData\\WithOutHeader.csv"), new Csv.Format(false));
        BigWithHeader = (new("TestData\\BigWithHeader.csv"), new Csv.Format());
        BigWithoutHeader = (new("TestData\\BigWithoutHeader.csv"), new Csv.Format(false)); 
    }

    #endregion
}