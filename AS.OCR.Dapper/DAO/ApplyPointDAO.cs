using AS.OCR.Dapper.Base;
using AS.OCR.IDAO;
using AS.OCR.Model.Business;
using AS.OCR.Model.Entity;
using System;
using System.Collections.Generic;

namespace AS.OCR.Dapper.DAO
{
    public class ApplyPointDAO : Infrastructure<ApplyPoint>, IApplyPointDAO
    {
        public List<ApplyPointModel> GetApplyPointHistory(Guid cardId)
        {
            var sql = $@"SELECT ap.ApplyPointID ApplyPointId,m.MallName,s.StoreName,ap.ReceiptPhoto Photo
, ap.TransDate,ap.ReceiptNo,ap.TransAmt,ap.[Status],ap.AddedOn,ap.RecongizeStatus,ap.VerifyStatus,ap.Remark
FROM ApplyPoint AS ap
LEFT JOIN Mall AS m ON ap.MallID = m.MallID
LEFT JOIN Store AS s ON ap.StoreID = s.StoreID AND s.[Enabled]=1
Where AP.CardId= '{cardId}' and AP.SourceType=7
";
            var result = GetListFromSql<ApplyPointModel>(sql);
            return result;
        }

        public int GetApplyPointCountByDay(Guid StoreId)
        {
            var sql = $@"select count(*) from ApplyPoint
                               where StoreID = '{StoreId}' and VerifyStatus = 1 
                               and SourceType=7 and DATEDIFF(dd,AuditDate,GETDATE())=0
";
            var result = GetModelFromSql<int>(sql);
            return result;
        }
    }
}
