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
        public List<ApplyPointModel> GetApplyPointHistory(string UnionId)
        {
            var sql = $@"SELECT ap.ApplyPointID ApplyPointId,m.MallName,s.StoreName,ap.ReceiptPhoto Photo
, ap.TransDate,ap.ReceiptNo,ap.TransAmt,ap.[Status],ap.AddedOn,ap.RecongizeStatus,ap.VerifyStatus,ap.Remark
FROM ApplyPoint AS ap
INNER JOIN Account AS m ON ap.MallID = m.MallID
LEFT JOIN Store AS s ON ap.StoreID = s.StoreID AND s.[Enabled]=1
Where AP.UnionId= '{UnionId}' and AP.SourceType=7 and m.AccountId='{AccountInfo.Account.Id}'
";
            return GetListFromSql<ApplyPointModel>(sql);
        }

        public int GetApplyPointCountByDay(Guid? StoreId)
        {
            var sql = $@"select count(*) from ApplyPoint
                               where StoreID = '{StoreId}' and VerifyStatus = 1 
                               and SourceType=7 and DATEDIFF(dd,AuditDate,GETDATE())=0
";
            var result = GetModelFromSql<int>(sql);
            return result;
        }

        public bool Exsist(string ReceiptNo, Guid? StoreId, DateTime? TransDatetime)
        => GetModel(@$" and ReceiptNo='{ReceiptNo}' 
and StoreID = '{StoreId}'                                                         
and TransDate = '{TransDatetime} ") == null;
    }
}
