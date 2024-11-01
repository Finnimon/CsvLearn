namespace Csv;

public readonly struct Format
{
    public Format(bool hasHeader = false, char separator = ',', string lineBreak = "\r\n", char regexEscape = '"')
    {
        RegexEscape = regexEscape;
        Separator = separator;
        LineBreak = lineBreak;
        HasHeader = hasHeader;
    }

    public char RegexEscape { get; }
    public char Separator { get; }
    public string LineBreak { get; }
    public bool HasHeader { get; }
}