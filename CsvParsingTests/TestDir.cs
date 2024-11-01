using Csv;

namespace CsvTests;

internal class TestDir
{
    #region Construction

    public TestDir()
    {
        Format headerFormat = new(true);
        var headerLessFormat = new Format(false);

        WithHeader = (new FileInfo("TestData\\WithHeader.csv"), headerFormat);
        WithOutHeader = (new FileInfo("TestData\\WithOutHeader.csv"), headerLessFormat);
        BigWithHeader = (new FileInfo("TestData\\BigWithHeader.csv"), headerFormat);
        BigWithoutHeader = (new FileInfo("TestData\\BigWithoutHeader.csv"), headerLessFormat);
    }

    #endregion

    #region Properties

    public (FileInfo file, Format format) WithHeader { get; }
    public (FileInfo file, Format format) WithOutHeader { get; }
    public (FileInfo file, Format format) BigWithHeader { get; }
    public (FileInfo file, Format format) BigWithoutHeader { get; }
    #endregion
}