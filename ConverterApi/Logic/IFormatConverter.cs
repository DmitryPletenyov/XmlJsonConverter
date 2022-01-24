using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConverterApi.Models;

namespace ConverterApi.Logic
{
    public interface IFormatConverter
    {
        public Document Deserialize(string input);
        public string Serialize(Document input);

        public string OutputFileName { get; }
    }
}
