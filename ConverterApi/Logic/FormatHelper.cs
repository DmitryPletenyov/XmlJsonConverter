using ConverterApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConverterApi.Logic
{
    public static class FormatHelper
    {
        public static IFormatConverter GetFormatConverter(Formats format )
        {
            switch (format)
            {
                case Formats.Xml:
                    // TODO: reuse?
                    return new XmlFormatConverter();
                    break;
                case Formats.Json:
                    return new JsonFormatConverter();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
                    break;
            }
        }

        public static Document Deserialize(Formats from, string input)
        {
            var inputConverter = GetFormatConverter(from);
            return inputConverter.Deserialize(input);
        }

        public static byte[] Serialize(Formats to, Document doc, out string outputFileName)
        {
            var outputConverter = GetFormatConverter(to);
            var serialized = outputConverter.Serialize(doc);
            outputFileName = outputConverter.OutputFileName;
            return System.Text.Encoding.ASCII.GetBytes(serialized);
        }
    }
}
