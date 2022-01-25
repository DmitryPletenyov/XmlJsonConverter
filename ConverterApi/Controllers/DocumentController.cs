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
using ConverterApi.Logic;
using Microsoft.AspNetCore.Hosting;

namespace ConverterApi.Controllers
{
    [ApiController]
    [Route("doc")]
    public class DocumentController : ControllerBase
    {
        private readonly ILogger<DocumentController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string contentType = "application/octet-stream";

        public DocumentController(ILogger<DocumentController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet()]
        public IActionResult Index()
        {
            return Ok();
        }

        [HttpPost()]
        [Route("Convert/{from}/{to}")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(bytes: 30_000_000)]
        public async Task<IActionResult> Convert(Formats from, Formats to, [FromQuery]string email, [FromForm] IFormFile file)
        {
            if (file == null) return BadRequest();
            if (from == to)
            {
                // TODO: return original input file?
                return BadRequest();
            }

            var input = string.Empty;
            using (var sr = new StreamReader(file.OpenReadStream()))
            {
                try
                {
                    input = await sr.ReadToEndAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error while reading input file", ex);
                    return BadRequest();
                }
            }

            Document doc = null;
            try
            {
                doc = FormatHelper.Deserialize(from, input);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while deserializing input to object", ex);
                return BadRequest();
            }

            string outputFileName = string.Empty;
            byte[] fileBytes = FormatHelper.Serialize(to, doc, out outputFileName);

            if (!string.IsNullOrEmpty(email))
            {
                try
                {
                    // https://localhost:44316/doc/Convert/xml/json?email=test@gmail.com
                    await EmailSender.Send(email, fileBytes, outputFileName, contentType);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error in email sending", ex);
                }
            }

            return new FileContentResult(fileBytes, contentType) { FileDownloadName = outputFileName };
        }

        [HttpGet()]
        [Route("Convert/{from}/{to}")]
        public async Task<IActionResult> ConvertByUrl(Formats from, Formats to, string fileUrl, string email)
        {
            // https://localhost:44316/doc/Convert/json/xml?fileUrl=https://localhost:44316/mock/json
            // https://localhost:44316/doc/Convert/xml/jsonfileUrl=https://localhost:44316/mock/xml

            if (string.IsNullOrWhiteSpace(fileUrl)) return BadRequest();
            if (from == to)
            {
                // TODO: return original input file?
                return BadRequest();
            }

            var client = new HttpClient();
            var response = await client.GetAsync(fileUrl);
            var input = string.Empty;

            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                using (var streamReader = new StreamReader(stream))
                {
                    input = await streamReader.ReadToEndAsync();
                }
            }

            if (string.IsNullOrWhiteSpace(input)) return BadRequest();

            Document doc = null;
            try
            {
                doc = FormatHelper.Deserialize(from, input);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while deserializing input to object", ex);
                return BadRequest();
            }

            string outputFileName = string.Empty;
            byte[] fileBytes = FormatHelper.Serialize(to, doc, out outputFileName);

            if (!string.IsNullOrEmpty(email))
            {
                try
                {
                    await EmailSender.Send(email, fileBytes, outputFileName, contentType);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error in email sending", ex);
                }
            }

            return new FileContentResult(fileBytes, contentType) { FileDownloadName = outputFileName };
        }

        [HttpGet()]
        [Route("ConvertLocalFile/{from}/{to}")]
        public async Task<IActionResult> ConvertByLocalPath(Formats from, Formats to, string filePath, string resultFilePath, string email)
        {
            // https://localhost:44316/doc/ConvertLocalFile/xml/json?filePath=SourceFiles\\Document1.xml&resultFilePath=TargetFiles

            if (string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(resultFilePath)) return BadRequest();
            if (from == to)
            {
                // TODO: return original input file?
                return BadRequest();
            }

            var readFrom = Path.Combine(_webHostEnvironment.ContentRootPath, filePath);
            string input = string.Empty;
            try
            {
                input = await System.IO.File.ReadAllTextAsync(readFrom);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while reading file {readFrom}", ex);
                return BadRequest();
            }

            if (string.IsNullOrWhiteSpace(input)) return BadRequest();

            Document doc = null;
            try
            {
                doc = FormatHelper.Deserialize(from, input);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while deserializing input to object", ex);
                return BadRequest();
            }

            string outputFileName = string.Empty;
            byte[] fileBytes = FormatHelper.Serialize(to, doc, out outputFileName);

            var saveTo = Path.Combine(_webHostEnvironment.ContentRootPath, $"{resultFilePath}//{outputFileName}");
            try
            {
                await System.IO.File.WriteAllBytesAsync(saveTo, fileBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while writing to file {saveTo}", ex);
                return BadRequest();
            }

            if (!string.IsNullOrEmpty(email))
            {
                try
                {
                    await EmailSender.Send(email, fileBytes, outputFileName, contentType);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error in email sending", ex);
                }
            }

            return new FileContentResult(fileBytes, contentType) { FileDownloadName = outputFileName };
        }
    }
}
