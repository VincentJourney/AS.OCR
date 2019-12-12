using AS.OCR.Dapper;
using AS.OCR.Dapper.Base;
using AS.OCR.Model.Business;
using AS.OCR.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AS.OCR.Dapper.DAO
{
    public class ApplyPointDAO : Infrastructure<ApplyPoint>
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
            var result = GetList<ApplyPointModel>(sql);
            return result;
        }
    }
}
