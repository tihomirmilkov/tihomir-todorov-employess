using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;

namespace tihomir_todorov_employess.Models
{
    public class FileModel : PageModel
    {
        [BindProperty]
        public SingleFileUpload FileUpload { get; set; }
    }

    public class SingleFileUpload
    {
        [Required]
        [Display(Name = "File")]
        public IFormFile FileData { get; set; }
    }
}