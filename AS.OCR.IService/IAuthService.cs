using AS.OCR.Model.Response;

namespace AS.OCR.IService
{
    public interface IAuthService
    {
        TokenResponse CreateToken(string appid, string appsecret);
    }
}
