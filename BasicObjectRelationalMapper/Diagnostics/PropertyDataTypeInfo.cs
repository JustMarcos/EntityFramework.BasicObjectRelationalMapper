using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicObjectRelationalMapper.Diagnostics
{
    public class PropertyDataTypeInfo
    {
        public PropertySource PropertyOrigin { get; private set; }
        public string PropertyName { get; private set; }
        public string DataTypeName { get; private set; }
        public bool IsNullableInCSharp { get; private set; }
        public bool DoesDbAllowsNull { get; private set; }


        public string NullableSymbol => GetNullableSymbol();
        public string DataType => $"{DataTypeName}{NullableSymbol}";
        public string DataTypeDescription => GetDataTypeDescription();

        /// <summary>
        /// Constructor for gathering DataType information for comparison.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="propertyName"></param>
        /// <param name="dataTypeName"></param>
        /// <param name="isNullable"></param>
        /// <param name="dbAllowNull"></param>
        public PropertyDataTypeInfo(PropertySource origin, string propertyName, string dataTypeName, bool isNullable, bool? dbAllowNull)
        {
            #region Checking parameters
            if (origin == PropertySource.DatabaseColumn && !dbAllowNull.HasValue)
            {
                throw new ArgumentNullException(nameof(dbAllowNull) + " is required for Database properties");
            }
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName) + " is required");
            }
            if (string.IsNullOrEmpty(dataTypeName))
            {
                throw new ArgumentNullException(nameof(dataTypeName) + " is required");
            }
            #endregion

            PropertyOrigin = origin;
            PropertyName = propertyName;
            DataTypeName = dataTypeName;
            IsNullableInCSharp = isNullable;
            DoesDbAllowsNull = dbAllowNull ?? false;
        }

        private string GetDataTypeDescription()
        {
            string info = $"{DataType} \t{PropertyName}";

            if (PropertyOrigin == PropertySource.DatabaseColumn)
            {
                if (DoesDbAllowsNull)
                {
                    info += "\t[Database Allows Null]";
                }
            }
            return info;
        }

        /// <summary>
        /// Determines if the nullable symbol "?" should be used as part of the Datatype.
        /// Example: The String object is already nullable and so a propety denoted as String? would not make sense.
        /// </summary>
        /// <returns></returns>
        private string GetNullableSymbol()
        {
            bool isStringOrByte = DataTypeName.Contains("String") || DataTypeName.Contains("Byte[]");
            return IsNullableInCSharp && !isStringOrByte ? "?" : string.Empty;

            //Note: A nullable byte? is possible, but it's not likely to be commonly used.
            //A much more common use is byte[] where the array can be set to null, and so byte[]? is not a valid syntax.
        }
    }

}
