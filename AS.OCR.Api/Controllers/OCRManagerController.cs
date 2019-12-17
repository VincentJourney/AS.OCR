using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AS.OCR.Api.Controllers
{
    public class OCRManagerController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/OCRManager/Index.cshtml");
        }

        public IActionResult GetList()
        {
            return Ok();
        }
    }
}