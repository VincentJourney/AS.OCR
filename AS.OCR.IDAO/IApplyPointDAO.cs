using AS.OCR.IDAO.Base;
using AS.OCR.Model.Business;
using AS.OCR.Model.Entity;
using System;
using System.Collections.Generic;

namespace AS.OCR.IDAO
{
    public interface IApplyPointDAO : IBaseDAO<ApplyPoint>
    {
        List<ApplyPointModel> GetApplyPointHistory(Guid cardId);

        int GetApplyPointCountByDay(Guid StoreId);
    }
}
