using NUnit.Framework;
using ConverterApi.Logic;
using ConverterApi.Models;
using System.Collections.Generic;

namespace ConverterApiTest
{
    [TestFixture]
    public class FormatHelperTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetJsonFormatConverter()
        {
            var r = FormatHelper.GetFormatConverter(Formats.Json);
            Assert.IsTrue(r.GetType() == typeof(JsonFormatConverter), "Json format should return JsonFormatConverter object");
        }

        [Test]
        public void GetXmlFormatConverter()
        {
            var r = FormatHelper.GetFormatConverter(Formats.Xml);
            Assert.IsTrue(r.GetType() == typeof(XmlFormatConverter), "Xml format should return XmlFormatConverter object");
        }

        public static IEnumerable<TestCaseData> DeserializeTestCases
        {
            get
            {
                yield return new TestCaseData(@"{""Title"":""Test title"",""Text"":""Test text""}", new Document() { Title="Test title", Text="Test text"}, new JsonFormatConverter());
                yield return new TestCaseData(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
<title>Test title</title>
<text>Test text</text>
</root>", new Document() { Title = "Test title", Text = "Test text" }, new XmlFormatConverter());
            }
        }

        [TestCaseSource("DeserializeTestCases")]
        public void DeserializeTest(string input, Document doc, IFormatConverter converter)
        {
            var r = converter.Deserialize(input);
            Assert.IsNotNull(r, "Deserialized object should not be null");
            Assert.IsTrue(r.Title == doc.Title, "Title property is not correct");
            Assert.IsTrue(r.Text == doc.Text, "Text property is not correct");
        }

        public static IEnumerable<TestCaseData> SerializeTestCases
        {
            get
            {
                yield return new TestCaseData(new Document() { Title = "Test title", Text = "Test text" }, @"{""Title"":""Test title"",""Text"":""Test text""}", new JsonFormatConverter());
                yield return new TestCaseData(new Document() { Title = "Test title", Text = "Test text" }, @"<?xml version=""1.0"" encoding=""utf-16""?>
<Root>
  <title>Test title</title>
  <text>Test text</text>
</Root>", new XmlFormatConverter());
                yield return new TestCaseData(null, @"{}", new JsonFormatConverter());
            }
        }

        [TestCaseSource("SerializeTestCases")]
        public void SerializeTest(Document doc, string output, IFormatConverter converter)
        {
            var r = converter.Serialize(doc);
            Assert.IsFalse(string.IsNullOrEmpty(r), "Serialized object should not be null or empty");
            Assert.IsTrue(r == output, "Serialized object is not correct");
        }
    }
}