using AS.OCR.Model.Response;
using Microsoft.AspNetCore.Mvc;

namespace AS.OCR.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        public IActionResult SuccessRes<T>(T Data = null, string Mes = "") where T : class
            => Ok(Result<T>.SuccessRes(Data, Mes));

        public IActionResult FailRes<T>(T ErrorMes = null, int code = 1001) where T : class
            => BadRequest(Result<string>.FailRes(ErrorMes?.ToString(), code));

    }
}