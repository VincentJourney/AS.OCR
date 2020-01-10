using AS.OCR.Model.Entity;
using System;

namespace AS.OCR.IService
{
    public interface IAccountService
    {
        Account GetByAppId(string AppId);
        Account Get(Guid Id);
    }
}
