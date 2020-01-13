using AS.OCR.IDAO.Base;
using AS.OCR.Model.Business;
using AS.OCR.Model.Entity;
using System;
using System.Collections.Generic;

namespace AS.OCR.IDAO
{
    public interface IApplyPointDAO : IBaseDAO<ApplyPoint>
    {
        List<ApplyPointModel> GetApplyPointHistory(string UnionId);

        int GetApplyPointCountByDay(Guid? StoreId);

        bool Exsist(string ReceiptNo, Guid? StoreId, DateTime? TransDatetime);
    }
}
