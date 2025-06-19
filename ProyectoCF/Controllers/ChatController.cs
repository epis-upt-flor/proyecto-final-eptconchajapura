using Microsoft.AspNetCore.Mvc;
using ProyectoCF.Models;
using System.Text;
using System.Text.Json;

namespace ProyectoCF.Controllers
{
    [Route("Chat")]
    public class ChatController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public ChatController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet("Index")]
        public IActionResult Index()
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Estudiante") return Forbid();

            return View();
        }

        [HttpPost("UploadPdf")]
        public async Task<IActionResult> UploadPdf(IFormFile pdfFile)
        {
            try
            {
                if (pdfFile == null || pdfFile.Length == 0)
                    return BadRequest(new { error = "Archivo inválido o vacío." });

                if (Path.GetExtension(pdfFile.FileName).ToLower() != ".pdf")
                    return BadRequest(new { error = "Solo se permiten archivos PDF." });

                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);

                var fileId = Guid.NewGuid().ToString();
                var filePath = Path.Combine(uploads, fileId + ".pdf");

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await pdfFile.CopyToAsync(stream);
                }

                return Json(new
                {
                    fileId,
                    fileName = pdfFile.FileName,
                    message = "PDF subido correctamente"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error al subir PDF: {ex.Message}" });
            }
        }

        [HttpPost("AskPdf")]
        public async Task<IActionResult> AskPdf([FromBody] PdfChatRequest request)
        {
            try
            {
                if (request?.FileIds == null || !request.FileIds.Any())
                    return BadRequest(new { error = "No se proporcionaron IDs de archivos." });

                if (string.IsNullOrWhiteSpace(request.Message))
                    return BadRequest(new { error = "El mensaje no puede estar vacío." });

                var apiKey = "AIzaSyCoiyJYcOxhL_Aeavbt4T0kSl9icIYCOiU";
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                var pdfParts = new List<object>();
                var validFiles = new List<string>();

                foreach (var fileId in request.FileIds)
                {
                    var filePath = Path.Combine(uploads, fileId + ".pdf");
                    if (System.IO.File.Exists(filePath))
                    {
                        validFiles.Add(fileId);
                        var pdfBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                        pdfParts.Add(new
                        {
                            inlineData = new
                            {
                                mimeType = "application/pdf",
                                data = Convert.ToBase64String(pdfBytes)
                            }
                        });
                    }
                }

                if (!pdfParts.Any())
                    return BadRequest(new { error = "Ninguno de los PDFs existe o es accesible." });

                pdfParts.Add(new { text = request.Message + " Responde en español sobre los documentos" });

                var payload = new
                {
                    contents = new[] { new { parts = pdfParts } }
                };

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);

                var response = await client.PostAsync(
                    "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent",
                    new StringContent(
                        JsonSerializer.Serialize(payload),
                        Encoding.UTF8,
                        "application/json"));

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonDocument>(json);

                return Json(new
                {
                    response = result.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString(),
                    filesUsados = validFiles
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = $"Error al procesar la solicitud: {ex.Message}",
                    detalle = ex.StackTrace
                });
            }
        }

        [HttpPost("RemovePdf")]
        public IActionResult RemovePdf([FromBody] RemovePdfRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FileId))
                return BadRequest("ID de archivo inválido.");

            var uploads = Path.Combine(_env.WebRootPath, "uploads");
            var filePath = Path.Combine(uploads, request.FileId + ".pdf");

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return Json(new { success = true });
        }
    }

    public class PdfChatRequest
    {
        public List<string>? FileIds { get; set; }
        public string? Message { get; set; }
    }

    public class RemovePdfRequest
    {
        public string? FileId { get; set; }
    }
}
