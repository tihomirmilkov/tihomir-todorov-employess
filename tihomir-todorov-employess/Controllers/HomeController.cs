﻿using System;
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

        [HttpPost]
        public async Task<IActionResult> UploadFileAsync(FileModel file)
        {
            if (file.FileUpload.FileData == null || file.FileUpload.FileData.Length <= 0)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
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
                var result = inputFileProcessor.ProcessFile(filePath);
            }
            catch (InvalidDataException ex)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest); 
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError); 
            }

            return Ok(); 
        }
    }
}
