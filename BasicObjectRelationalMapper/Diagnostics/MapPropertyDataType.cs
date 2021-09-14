using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicObjectRelationalMapper.Diagnostics
{
    public class MapPropertyDataType : IMapPropertyDataType
    {
        /// <summary>
        /// Uses Reflection to gather DataType information from a C# Class
        /// </summary>
        /// <param name="t"></param>
        /// <returns>Returns List<PropertyDataTypeInfo></returns>
        public List<PropertyDataTypeInfo> GetClassProperties(Type t)
        {
            //string className = t.Name;
            var classProperties = new List<PropertyDataTypeInfo>();

            //CLASS PROPERTIES --| DATA TYPE | ISNULLABLE | PROPERTY NAME |
            foreach (var p in t.GetProperties())
            {
                var propertyType = p.PropertyType;
                string propertyName = p.Name;
                bool isNullable = Nullable.GetUnderlyingType(propertyType) != null;
                string dataType = isNullable ? propertyType.GenericTypeArguments.First().FullName : propertyType.FullName; 

                var propertyDescription = new PropertyDataTypeInfo(PropertySource.CSharpClass, propertyName, dataType, isNullable, null);
                classProperties.Add(propertyDescription);
            }

            return classProperties;
        }

        /// <summary>
        /// Loops through the SqlDataReader to gather DataType information for each column.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>Returns List<PropertyDataTypeInfo></returns>
        public List<PropertyDataTypeInfo> GetDataTableProperties(System.Data.SqlClient.SqlDataReader reader)
        {
            var dataTable = new System.Data.DataTable();
            dataTable.Load(reader);

            var dataTableProperties = new List<PropertyDataTypeInfo>();

            //SQL DATA READER PROPERTIES --| DATA TYPE | ISNULLABLE | COLUMN NAME |
            foreach (System.Data.DataColumn c in dataTable.Columns)
            {
                var propertyDescription = new PropertyDataTypeInfo(PropertySource.DatabaseColumn, c.ColumnName, c.DataType.FullName, c.AllowDBNull, c.AllowDBNull);
                dataTableProperties.Add(propertyDescription);
            }

            return dataTableProperties;
        }

    }
}
