using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BasicObjectRelationalMapper.Diagnostics
{
    public interface IMapPropertyDataType
    {
        List<PropertyDataTypeInfo> GetClassProperties(Type t);
        List<PropertyDataTypeInfo> GetDataTableProperties(SqlDataReader reader);
    }
}