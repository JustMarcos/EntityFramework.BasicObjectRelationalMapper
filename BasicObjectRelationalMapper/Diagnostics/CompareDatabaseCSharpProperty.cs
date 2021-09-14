using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicObjectRelationalMapper.Diagnostics
{
    public class CompareDatabaseCSharpProperty
    {
        public string PropertyName { get; set; }
        public string DataTypeFromClass { get; set; }
        public string DataTypeFromDatabase { get; set; }
        public bool DoesDatabaseAllowsNull { get; set; }

        public string DatabaseNullableText => DoesDatabaseAllowsNull ? "\t[Database Allows Null]" : "\t[Database Value Cannot Be Null]";
        public bool IsMatch => DataTypeFromDatabase == DataTypeFromClass;
        private string MatchText => IsMatch ? "Match" : "Mismatch";
        public string ResultText => $"{MatchText} DataType => Database: {DataTypeFromDatabase} | Class: {DataTypeFromClass}  \t{PropertyName} {DatabaseNullableText}";
    }
}
