using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using ConverterApi.Models;
using System.Net.Http;

namespace ConverterApi.Controllers
{
#if DEBUG

    [ApiController]
    [Route("mock")]
    public class MockController : ControllerBase
    {
        [HttpGet()]
        [Route("{format}")]
        public IActionResult Mock(string format)
        {
            if (string.IsNullOrEmpty(format) || format.ToLower() == "xml")
            {
                return Ok(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<root>
<title>Some title</title>
<text>Some text</text>
</root>");
            }

            return Ok(@"{""Title"":""Some title"",""Text"":""Some text""}");
        }
    }

#endif
}
