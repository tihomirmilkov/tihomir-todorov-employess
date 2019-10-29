using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tihomir_todorov_employess.Models;
using tihomir_todorov_employess.Utilities;

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

        public async Task<IActionResult> UploadFileAsync(FileModel file)
        {
            var result = new ResultModel();

            // check if file is empty
            if (file.FileUpload.FileData == null || file.FileUpload.FileData.Length <= 0)
            {
                result.ErrorMessage = "Empty file";
                return View("Index", result);
            }

            // check if file is a ".txt" file - task requirement
            if (!Path.GetExtension(file.FileUpload.FileData.FileName).Equals(".txt", StringComparison.OrdinalIgnoreCase))
            {
                result.ErrorMessage = "File should be .txt";
                return View("Index", result);
            }

            try
            {
                // download file in temp location
                var filePath = Path.GetTempFileName();
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.FileUpload.FileData.CopyToAsync(stream);
                }

                // process uploaded file - use temporary saved location
                var inputFileProcessor = new InputFileProcessor();
                result.employeesCouples = inputFileProcessor.ProcessFile(filePath);
            }
            catch (InvalidDataException ex)
            {
                result.ErrorMessage = "Incorrect data in file. Error while parsing.";
                return View("Index", result);
            }
            catch (Exception ex)
            {
                result.ErrorMessage = "Oops. Something went wrong on server side.";
                return View("Index", result);
            }

            return View("Index", result);
        }
    }
}
