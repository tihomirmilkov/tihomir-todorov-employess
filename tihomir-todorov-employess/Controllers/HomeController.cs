using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tihomir_todorov_employess.Models;

namespace tihomir_todorov_employess.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> UploadFileAsync(FileModel file)
        {
            if (file.FileUpload.FileData == null || file.FileUpload.FileData.Length <= 0)
            {
                return BadRequest(); // 400
            }

            try
            {
                // download file in temp location
                var filePath = Path.GetTempFileName();
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.FileUpload.FileData.CopyToAsync(stream);
                }

                // process uploaded file
                // Don't rely on or trust the FileName property without validation.

            }
            catch (Exception ex)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError); // 500
            }

            return Ok(); // 200
        }
    }
}
