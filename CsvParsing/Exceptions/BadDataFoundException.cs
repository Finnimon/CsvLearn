namespace Csv.Exceptions;

public class BadDataFoundException : CsvException
{
    private BadDataFoundException(string msg) : base(msg)
    {
    }

    public static BadDataFoundException BadRecordLengthException(int expected, int read) => new("");
}