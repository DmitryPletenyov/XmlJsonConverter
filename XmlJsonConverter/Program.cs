using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
//using Newtonsoft.Json;
using System.Text.Json;

namespace XmlJsonConverter
{
    public class Document
    {
        public string Title { get; set; }
        public string Text { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //var sourceFileName = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\Source Files\\Document1.xml");
            //var targetFileName = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\Target Files\\Document1.json");

            var sourceFileName = Path.GetFullPath("..\\..\\..\\Source Files\\Document1.xml", Environment.CurrentDirectory);
            var targetFileName = Path.GetFullPath("..\\..\\..\\Target Files\\Document1.json", Environment.CurrentDirectory);

            string input = string.Empty;
            FileStream sourceStream = null;
            StreamReader reader = null;
            try
            {
                /*FileStream*/ sourceStream = File.Open(sourceFileName, FileMode.Open);
                /*var*/ reader = new StreamReader(sourceStream);
                /*string*/ input = reader.ReadToEnd();
            }
            // TODO: add catch block for more specific ex
            catch (Exception ex)
            {
                // TODO: add logging
                throw; // keep the original ex with call stack
                //throw new Exception(ex.Message);
            }
            finally
            {
                sourceStream?.Dispose();
                reader?.Dispose();
            }

            var xdoc = XDocument.Parse(input);
            var doc = new Document
            {
                Title = xdoc.Root.Element("title").Value,
                Text = xdoc.Root.Element("text").Value
            };

            //var serializedDoc = JsonConvert.SerializeObject(doc);
            var serializedDoc = JsonSerializer.Serialize(doc);

            using (var targetStream = File.Open(targetFileName, FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(targetStream))
                {
                    sw.Write(serializedDoc);
                }

        }
    }
}
