using BasicObjectRelationalMapper.Diagnostics;
using BasicObjectRelationalMapper.ORM;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicObjectRelationalMapper.Modeling
{
    public class ModelGenerator : IModelGenerator
    {
        private readonly string _connectionString;
        private readonly IMapPropertyDataType _mapper;

        public ModelGenerator(string connectionString)
        {
            _connectionString = connectionString;
            _mapper = new MapPropertyDataType();
        }

        /// <summary>
        /// Reverse Engineers the given stored procedure to the PocoDataClass object which can be used to create the POCO class.
        /// </summary>
        /// <param name="storedProcedureInfo"></param>
        /// <returns></returns>
        public PocoDataClass GetPocoDataClass(StoredProcedureInfo storedProcedureInfo)
        {
            PocoDataClass pocoInfo = null;

            using (var _context = new DatabaseContext(_connectionString))
            {
                var cmd = _context.CreateSqlCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = storedProcedureInfo.StoredProcedureName;
                foreach (var p in storedProcedureInfo.SqlParameters)
                {
                    cmd.Parameters.Add(p);
                }

                cmd.Connection.Open();

                var reader = cmd.ExecuteReader();
                var properties = _mapper.GetDataTableProperties(reader);

                pocoInfo = new PocoDataClass() { NameOfClassFile = storedProcedureInfo.ClassName, Properties = properties };
            }

            return pocoInfo;
        }

        /// <summary>
        /// Reverse Engineers a list of stored procedures to a list of PocoDataClass objects where each can be used to create a POCO class.
        /// </summary>
        /// <param name="storedProcedureList"></param>
        /// <returns></returns>
        public List<PocoDataClass> GetPocoDataClasses(IEnumerable<StoredProcedureInfo> storedProcedureList)
        {
            var list = new List<PocoDataClass>();

            //Note: Uses the same context instead of creating a new context for each loop.
            using (var _context = new DatabaseContext(_connectionString))
            {
                foreach (var s in storedProcedureList)
                {
                    var cmd = _context.CreateSqlCommand();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = s.StoredProcedureName;

                    foreach (var p in s.SqlParameters)
                    {
                        cmd.Parameters.Add(p);
                    }

                    cmd.Connection.Open();

                    var reader = cmd.ExecuteReader();
                    var properties = _mapper.GetDataTableProperties(reader);

                    list.Add(new PocoDataClass { NameOfClassFile = s.ClassName, Properties = properties });
                }
            }

            return list;
        }

        //TODO: Possibly create methods to map SQL Queries. In other words, SELECT statements. 

    }
}
