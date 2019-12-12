﻿using System;

namespace AS.OCR.Model.Response
{
    public class Result
    {
        public Result(bool Success = false, string Messages = "", Object Data = null)
        {
            this.Success = Success;
            this.Message = Messages;
            this.Data = Data;
        }
        public string Message { get; set; }
        public Object Data { get; set; }
        public bool Success { get; set; }

        public static Result SuccessRes(string mes = "", Object data = null) => new Result(true, mes, data);
        public static Result ErrorRes(string mes = "", Object data = null) => new Result(false, mes, data);

    }
}
