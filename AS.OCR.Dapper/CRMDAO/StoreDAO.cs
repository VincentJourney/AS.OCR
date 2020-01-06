using AS.OCR.Dapper;
using AS.OCR.Model.Business;
using AS.OCR.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using AS.OCR.Dapper.Base;

namespace AS.OCR.Dapper.DAO
{
    public class StoreDAO : Infrastructure<Store>
    {
        public Store GetById(string StoreId, string MallId)
        => GetModel($"StoreId = '{StoreId}' and mallId = '{MallId}' and Enabled = 1)");


    }
}
