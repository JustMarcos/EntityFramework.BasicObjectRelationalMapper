using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicObjectRelationalMapper.Diagnostics
{
    public class PropertyValidator
    {
        private readonly IEnumerable<PropertyDataTypeInfo> ClassProperties;
        private readonly IEnumerable<PropertyDataTypeInfo> DataTableProperties;
        private readonly string ClassName = "";
        private readonly string SqlDataReaderName = "";

        //CONSTRUCTOR
        public PropertyValidator(Type t, System.Data.SqlClient.SqlDataReader reader, string dataReaderName)
        {
            var mapper = new MapPropertyDataType();

            var dataTableProperties = mapper.GetDataTableProperties(reader);
            var classProperties = mapper.GetClassProperties(t);

            ClassProperties = classProperties;
            DataTableProperties = dataTableProperties;
            ClassName = t.Name;
            SqlDataReaderName = dataReaderName;
        }

        /// <summary>
        /// Compares the properties from the Database againt a C# class using Reflection by matching with the PropertyName. Matching the PropertyName is Case-Sensitive!
        /// It list properties that have their datatypes that are mismatched, matched, or not used (unmapped).
        /// </summary>
        /// <returns>ComparisonResult object</returns>
        public ComparisonResult Validate()
        {
            //Sort the [PropertyName] into groups.  
            var unmatchedDataTablePropertyNames = DataTableProperties.Select(x => x.PropertyName).Except(ClassProperties.Select(x => x.PropertyName)).OrderBy(o => o).ToList();
            var unmatchedClassPropertyNames = ClassProperties.Select(x => x.PropertyName).Except(DataTableProperties.Select(x => x.PropertyName)).OrderBy(o => o).ToList();
            var matchPropertyNames = ClassProperties.Select(x => x.PropertyName).Intersect(DataTableProperties.Select(x => x.PropertyName)).OrderBy(o => o).ToList();

            //Matches properties by [PropertyName] to compare the datatypes between the Database and C#
            //PROPERTY NAME IS CASE SENSITIVE! 
            var matchList = DataTableProperties.Concat(ClassProperties).Where(x => matchPropertyNames.Contains(x.PropertyName)).ToList();
            var matchResults = matchList.GroupBy(g => g.PropertyName)
                .Select(x =>
                {
                    var dbProp = x.Single(a => a.PropertyOrigin == PropertySource.DatabaseColumn);
                    var classProp = x.Single(a => a.PropertyOrigin == PropertySource.CSharpClass);
                    return new CompareDatabaseCSharpProperty
                    {
                        PropertyName = x.Key,
                        DataTypeFromClass = classProp.DataType,
                        DataTypeFromDatabase = dbProp.DataType,
                        DoesDatabaseAllowsNull = dbProp.DoesDbAllowsNull
                    };
                })
                .ToList();

            //Results
            var misMatched = matchResults.FindAll(x => !x.IsMatch);
            var matched = matchResults.FindAll(x => x.IsMatch);
            var unmappedFromDatabase = DataTableProperties.Where(x => unmatchedDataTablePropertyNames.Contains(x.PropertyName)).ToList();
            var unmappedFromClass = ClassProperties.Where(x => unmatchedClassPropertyNames.Contains(x.PropertyName)).ToList();

            var result = new ComparisonResult(misMatched, matched, unmappedFromDatabase, unmappedFromClass, ClassName, SqlDataReaderName );
            return result;
        }

    }
}
