using AS.OCR.Service;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AS.OCR.Model.Request;
using AS.OCR.IDAO;
using AS.OCR.Dapper;

namespace AS.OCR.Service.Tests
{
    [TestClass()]
    public class UnitTestAuthService
    {
        private AuthService authService { get; }
        public UnitTestAuthService()
        {
            IAccountDAO accountDAO = new AccountDAO();
            ILoggerFactory loggerFactory = new LoggerFactory();
            authService = new AuthService(loggerFactory, accountDAO);
        }
        [TestMethod()]
        public void CreateToken()
        {
            var token = authService.CreateToken("123", "123");
        }
    }
}

