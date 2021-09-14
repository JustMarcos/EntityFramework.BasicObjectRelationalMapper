using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicObjectRelationalMapper.Diagnostics
{
    public class ComparisonResult
    {
        public string ClassName { get; set; }
        public string SqlDataReaderName { get; set; }
        public List<CompareDatabaseCSharpProperty> MismatchedDataTypeProperties { get; private set; } = new List<CompareDatabaseCSharpProperty>();
        public List<CompareDatabaseCSharpProperty> MatchedDataTypeProperties { get; private set; } = new List<CompareDatabaseCSharpProperty>();
        public List<PropertyDataTypeInfo> UnmappedDatabaseProperties { get; private set; } = new List<PropertyDataTypeInfo>();
        public List<PropertyDataTypeInfo> UnmappedClassProperties { get; private set; } = new List<PropertyDataTypeInfo>();

        //CONSTRUCTOR
        public ComparisonResult(
            List<CompareDatabaseCSharpProperty> mismatchcedProperties, 
            List<CompareDatabaseCSharpProperty> matchedProperties, 
            List<PropertyDataTypeInfo> unmappedDataTableProperties, 
            List<PropertyDataTypeInfo> unmappedClassProperties, 
            string className, 
            string dataReaderName)
        {
            MismatchedDataTypeProperties = mismatchcedProperties;
            MatchedDataTypeProperties = matchedProperties;
            UnmappedDatabaseProperties = unmappedDataTableProperties;
            UnmappedClassProperties = unmappedClassProperties;
            ClassName = className;
            SqlDataReaderName = dataReaderName;
        }

        /// <summary>
        /// Mismatched DataType Results. Each result is in a string line.
        /// </summary>
        /// <returns></returns>
        public List<string> MismatchedResults()
        {
            var lines = new List<string>();
            lines.AddRange(MismatchedDataTypeProperties.Select(x => x.ResultText));
            return lines;
        }

        /// <summary>
        ///  Matched DataType Results. Each result is in a string line.
        /// </summary>
        /// <returns></returns>
        public List<string> MatchedResults()
        {
            var lines = new List<string>();
            lines.AddRange(MatchedDataTypeProperties.Select(x => x.ResultText));
            return lines;
        }

        /// <summary>
        ///  Unmapped Database properties Results. Each result is in a string line. Represents propeties not used.
        /// </summary>
        /// <returns></returns>
        public List<string> GetUnmappedDatabaseProperties()
        {
            var lines = new List<string>();
            lines.AddRange(UnmappedDatabaseProperties.Select(x => x.DataTypeDescription));
            return lines;
        }

        /// <summary>
        /// Unmapped Class properties Results. Each result is in a string line. Represents propeties not used.
        /// </summary>
        /// <returns></returns>
        public List<string> GetUnmappedClassProperties()
        {
            var lines = new List<string>();
            lines.AddRange(UnmappedClassProperties.Select(x => x.DataTypeDescription));
            return lines;
        }

        /// <summary>
        /// Print out the full results for: Mismatched, Matched, Unmapped Database, Unmapped Class properties
        /// </summary>
        /// <returns></returns>
        public string PrintOutFullResults()
        {
            var logInfo = new StringBuilder();

            logInfo.AppendLine();
            logInfo.AppendLine("Mismatches:");
            MismatchedResults().ForEach(x => logInfo.AppendLine(x));

            logInfo.AppendLine();
            logInfo.AppendLine("Matches:");
            MatchedResults().ForEach(x => logInfo.AppendLine(x));


            logInfo.AppendLine();
            logInfo.AppendLine("Unmapped Database Properties:");
            GetUnmappedDatabaseProperties().ForEach(x => logInfo.AppendLine(x));

            logInfo.AppendLine();
            logInfo.AppendLine("Unmapped Class Properties:");
            GetUnmappedClassProperties().ForEach(x => logInfo.AppendLine(x));

            logInfo.AppendLine();
            logInfo.AppendLine($"Class: {ClassName}.cs\nDatabase: {SqlDataReaderName}");

            string diagnostic = logInfo.ToString();
            return diagnostic;
        }


    }
}
