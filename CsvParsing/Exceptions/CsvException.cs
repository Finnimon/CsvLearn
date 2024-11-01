namespace Csv.Exceptions;

/// <summary>
///     Thrown for Csv related Exceptions
/// </summary>
public abstract class CsvException(string msg) : Exception(msg);