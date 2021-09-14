using BasicObjectRelationalMapper.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicObjectRelationalMapper.Modeling
{
    public class PocoDataClass
    {
        public string StoredProcedureName { get; set; }
        public string NameOfClassFile {get; set;}
        public List<PropertyDataTypeInfo> Properties { get; set; }

        /// <summary>
        /// Builds the POCO class as a string.
        /// </summary>
        /// <returns></returns>
        public string GeneratePocoClass()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("public class " + NameOfClassFile);
            stringBuilder.AppendLine("{");
            foreach (var c in Properties)
            {
                string line = $"\tpublic {c.DataTypeName}{c.NullableSymbol} {c.PropertyName}" + " { get; set; }";
                stringBuilder.AppendLine(line);
            }
            stringBuilder.AppendLine("}");

            return stringBuilder.ToString();
        }
    }
}
