using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConverterApi.Models;
using System.Text.Json;

namespace ConverterApi.Logic
{
    public class JsonFormatConverter : IFormatConverter
    {
        public Document Deserialize(string input)
        {
            return JsonSerializer.Deserialize<Document>(input);
        }

        public string Serialize(Document input)
        {
            if (input == null) return "{}";
            return JsonSerializer.Serialize(input);
        }

        public string OutputFileName {get { return "output.json"; } }
    }
}
