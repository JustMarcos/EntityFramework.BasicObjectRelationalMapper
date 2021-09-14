using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicObjectRelationalMapper.Modeling
{
    public class StoredProcedureInfo
    {
        public string StoredProcedureName { get; set; }
        public IEnumerable<SqlParameter> SqlParameters { get; set; } = new List<SqlParameter>();
        public string ClassName { get; set; }
    }
}
