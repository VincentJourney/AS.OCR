using Microsoft.AspNetCore.Mvc;
using System;

namespace AS.OCR.Model.Response
{
    public class Result<T> where T : class
    {
        public Result(T Data = null, bool Success = false, string Messages = "")
        {
            this.Success = Success;
            this.Message = Messages;
            this.Data = Data;
        }
        public string Message { get; set; }
        public T Data { get; set; }
        public bool Success { get; set; }

        public static Result<T> SuccessRes(T data, string mes = "") => new Result<T>(data, true, mes);
        public static Result<T> ErrorRes(string mes = "") => new Result<T>(Messages: mes);



    }
}
