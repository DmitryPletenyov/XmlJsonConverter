using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConverterApi.Models;
using System.Xml.Linq;
using System.Text;
using System.IO;

namespace ConverterApi.Logic
{
    public class XmlFormatConverter: IFormatConverter
    {
        public Document Deserialize(string input) 
        {
            var xdoc = XDocument.Parse(input);
            var doc = new Document
            {
                Title = xdoc.Root.Element("title").Value,
                Text = xdoc.Root.Element("text").Value
            };
            return doc;
        }

        public string Serialize(Document input)
        {
            StringBuilder sb = new StringBuilder();

            if (input == null) return string.Empty; // define up to business logic

            XDocument doc = new XDocument(
                    new XElement("Root",
                        new XElement("title", input.Title),
                        new XElement("text", input.Text)
                    )
            );

            using (TextWriter tr = new StringWriter(sb))
            {
                doc.Save(tr);
            }

            return sb.ToString();
        }

        public string OutputFileName { get { return "output.xml"; } }
    }
}
