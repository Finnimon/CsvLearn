using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csv;

namespace CsvTests
{
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
            WithHeader = (new("\\TestData\\WithHeader.csv"),new Format());
            WithOutHeader = (new("\\TestData\\WithOutHeader.csv"), new Format());
            BigWithHeader = (new("\\TestData\\BigWithHeader.csv"), new Format());
            BigWithoutHeader = (new("\\TestData\\BigWithoutHeader.csv"), new Format());
        }

        #endregion
    }
}
