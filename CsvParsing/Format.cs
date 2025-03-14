namespace Csv;

public readonly record struct Format(
    bool HasHeader = false,
    char Separator = ',',
    string LineBreak = "\r\n",
    char RegexEscape = '"');