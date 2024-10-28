using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csv;

public struct Format
{
    public Format(bool hasHeader=false, string separator=",", string newLine="\r\n")
    {
        HasHeader = hasHeader;
        Separator = separator;
        NewLine = newLine;
    }

    public string Separator { get; }
    public string NewLine { get; }
    public bool HasHeader { get; }
}