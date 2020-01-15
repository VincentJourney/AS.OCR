namespace AS.OCR.Model.Response
{
    public class Result<T>
    {
        public string Message { get; set; } = "";
        public T Data { get; set; } = default;
        public int Code { get; set; } = (int)ResultEnum.ExceptionError;

        public static Result<T> SuccessRes(T data, string mes = "")
            => new Result<T> { Data = data, Code = (int)ResultEnum.Success };

        public static Result<T> FailRes(string mes = "", int code = (int)ResultEnum.BusinessError)
            => new Result<T> { Message = mes, Code = code };
    }

    public enum ResultEnum
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 1000,
        /// <summary>
        /// 业务异常
        /// </summary>
        BusinessError,
        /// <summary>
        /// 程序异常
        /// </summary>
        ExceptionError,
    }

    public static class ResultExtension
    {
        public static bool HasError<T>(this Result<T> result) => result.Code != (int)ResultEnum.Success;
    }
}
