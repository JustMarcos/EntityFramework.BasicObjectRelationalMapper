using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace BasicObjectRelationalMapper.ORM
{
    public interface IDatabaseContext
    {
        void CloseConnection();
        SqlCommand CreateSqlCommand();
        void ExecuteNonQueryStoredProcedure(string storedProcedureName, IEnumerable<SqlParameter> sqlParameters = null);
        void ExecuteNonQueryStoredProcedure(string storedProcedureName, Dictionary<string, object> sqlParametersDictionary);
        Task ExecuteNonQueryStoredProcedureAsync(string storedProcedureName, IEnumerable<SqlParameter> sqlParameters = null);
        Task ExecuteNonQueryStoredProcedureAsync(string storedProcedureName, Dictionary<string, object> sqlParametersDictionary);
        List<T> MapToModel<T>(string storedProcedureName, Dictionary<string, object> sqlParamsDictionary) where T : DatabaseModelAbstractBase;
        List<T> MapToModel<T>(string storedProcedureName, IEnumerable<SqlParameter> sqlparams) where T : DatabaseModelAbstractBase;
        Task<List<T>> MapToModelAsync<T>(string storedProcedureName, Dictionary<string, object> sqlParamsDictionary) where T : DatabaseModelAbstractBase;
        Task<List<T>> MapToModelAsync<T>(string storedProcedureName, IEnumerable<SqlParameter> sqlparams) where T : DatabaseModelAbstractBase;
        List<T> MapToModelFromSqlQuery<T>(string rawSqlQuery) where T : DatabaseModelAbstractBase;
        Task<List<T>> MapToModelFromSqlQueryAsync<T>(string rawSqlQuery) where T : DatabaseModelAbstractBase;
    }
}