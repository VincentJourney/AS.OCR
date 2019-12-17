using AS.OCR.Commom.Util;
using System;
using System.Data;
using System.Data.SqlClient;

namespace AS.OCR.Dapper.Base.DapperHelper
{
    public class OcrDapperHelper : AbstractDapperHelper
    {
        public OcrDapperHelper(string ConnectionString) : base(ConnectionString)
        {
            base._connection = ConnectionString;
        }
    }
}
