using BasicObjectRelationalMapper.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicObjectRelationalMapper.ORM
{
    public class DatabaseContext : DbContext, IDatabaseContext
    {
        /// <summary>
        /// Constructor, calls the base constructor of DBContext
        /// </summary>
        public DatabaseContext(string connectonString) : base(connectonString) { }

        /// <summary>
        /// Creates a SQL Command object
        /// </summary>
        /// <returns>SqlCommand</returns>
        public SqlCommand CreateSqlCommand()
        {
            return this.Database.Connection != null ? this.Database.Connection.CreateCommand() as SqlCommand : null;
        }

        /// <summary>
        /// Closes Connection to the Database
        /// </summary>
        public void CloseConnection()
        {
            if (this.Database.Connection != null)
            {
                this.Database.Connection.Close();
            }
        }


        /// <summary>
        /// Maps the data from a SqlDataReader object to properties in a class <T> by matching column names to the property name.
        /// Class <T> must inherit from the base class {DatabaseModelAbstractBase}.
        /// </summary>
        /// <typeparam name="T">Type T must inherit from the abstract class.</typeparam>
        /// <param name="reader">SqlDataReader object</param>
        /// <returns>Returns List of type T</returns>
        private List<T> MapToModel<T>(SqlDataReader reader, string storedProcedureNameOrQuery) where T : DatabaseModelAbstractBase
        {
            try
            {
                var mapped = ((IObjectContextAdapter)this).ObjectContext.Translate<T>(reader);
                return mapped.ToList();
            }
            catch (Exception ex)
            {
                var validator = new PropertyValidator(typeof(T), reader, storedProcedureNameOrQuery);
                var result = validator.Validate();
                string exceptionMessage = $"{ex.Message}\n{string.Join("\n", result.MismatchedResults())}\n";

                throw new Exception(exceptionMessage, ex);
            }
        }


        /// <summary>
        /// Maps data to a list of generic type <T>
        /// </summary>
        /// <typeparam name="T">Type T must inherit from the abstract class.</typeparam>
        /// <param name="storedProcedureName">Name of Stored Procedure</param>
        /// <param name="sqlparams">Sql Parameters</param>
        /// <returns></returns>
        public List<T> MapToModel<T>(string storedProcedureName, IEnumerable<SqlParameter> sqlparams) where T : DatabaseModelAbstractBase
        {
            List<T> list = new List<T>();
            try
            {
                using (var cmd = CreateSqlCommand())
                {
                    cmd.CommandText = storedProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach (var p in sqlparams)
                    {
                        cmd.Parameters.Add(p);
                    }

                    cmd.Connection.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            var records = MapToModel<T>(reader, storedProcedureName);
                            list = records.ToList();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(FormatStoredProcedureError(storedProcedureName, sqlparams, ex.Message), ex);
            }
            finally
            {
                CloseConnection();
            }

            return list;
        }

        /// <summary>
        /// Maps data to a list of generic type T. Runs Asynchronously.
        /// </summary>
        /// <typeparam name="T">Type T must inherit from the abstract class.</typeparam>
        /// <param name="storedProcedureName">Name of Stored Procedure</param>
        /// <param name="sqlparams">Sql Parameters</param>
        /// <returns></returns>
        public async Task<List<T>> MapToModelAsync<T>(string storedProcedureName, IEnumerable<SqlParameter> sqlparams) where T : DatabaseModelAbstractBase
        {
            List<T> list = new List<T>();
            try
            {
                using (var cmd = CreateSqlCommand())
                {
                    cmd.CommandText = storedProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach (var p in sqlparams)
                    {
                        cmd.Parameters.Add(p);
                    }

                    cmd.Connection.Open();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            var records = MapToModel<T>(reader, storedProcedureName);
                            list = records.ToList();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(FormatStoredProcedureError(storedProcedureName, sqlparams, ex.Message), ex);
            }
            finally
            {
                CloseConnection();
            }

            return list;
        }


        /// <summary>
        /// Maps data to a list of generic type T. Use a Dictionary instead of using SqlParameter.
        /// </summary>
        /// <typeparam name="T">Type T must inherit from the abstract class.</typeparam>
        /// <param name="storedProcedureName"></param>
        /// <param name="sqlParamsDictionary"></param>
        /// <returns></returns>
        public List<T> MapToModel<T>(string storedProcedureName, Dictionary<string, object> sqlParamsDictionary) where T : DatabaseModelAbstractBase
        {
            var sqlParams = sqlParamsDictionary.Select(x => new SqlParameter(x.Key, x.Value)).ToArray();
            return MapToModel<T>(storedProcedureName, sqlParams);
        }

        /// <summary>
        /// Maps data to a list of generic type T. Use a Dictionary instead of using SqlParameter. Runs Asynchronously.
        /// </summary>
        /// <typeparam name="T">Type T must inherit from the abstract class.</typeparam>
        /// <param name="storedProcedureName"></param>
        /// <param name="sqlParamsDictionary"></param>
        /// <returns></returns>
        public async Task<List<T>> MapToModelAsync<T>(string storedProcedureName, Dictionary<string, object> sqlParamsDictionary) where T : DatabaseModelAbstractBase
        {
            var sqlParams = sqlParamsDictionary.Select(x => new SqlParameter(x.Key, x.Value)).ToArray();
            return await MapToModelAsync<T>(storedProcedureName, sqlParams);
        }


        /// <summary>
        /// Map data to a list of generic type T from a SQL query. 
        /// </summary>
        /// <typeparam name="T">Type T must inherit from the abstract class.</typeparam>
        /// <param name="rawSqlQuery"></param>
        /// <returns></returns>
        public List<T> MapToModelFromSqlQuery<T>(string rawSqlQuery) where T : DatabaseModelAbstractBase
        {

            List<T> list = new List<T>();
            try
            {
                using (var cmd = CreateSqlCommand())
                {
                    cmd.CommandText = rawSqlQuery;
                    cmd.CommandType = CommandType.Text;

                    cmd.Connection.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            var records = MapToModel<T>(reader, rawSqlQuery);
                            list = records.ToList();
                        }
                    }
                }

            }
            finally
            {
                CloseConnection();
            }

            return list;
        }

        /// <summary>
        /// Map data to a list of generic type T from a SQL query. Runs Asynchronously.
        /// </summary>
        /// <typeparam name="T">Type T must inherit from the abstract class.</typeparam>
        /// <param name="rawSqlQuery"></param>
        /// <returns></returns>
        public async Task<List<T>> MapToModelFromSqlQueryAsync<T>(string rawSqlQuery) where T : DatabaseModelAbstractBase
        {

            List<T> list = new List<T>();
            try
            {
                using (var cmd = CreateSqlCommand())
                {
                    cmd.CommandText = rawSqlQuery;
                    cmd.CommandType = CommandType.Text;

                    cmd.Connection.Open();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            var records = MapToModel<T>(reader, rawSqlQuery);
                            list = records.ToList();
                        }
                    }
                }

            }
            finally
            {
                CloseConnection();
            }

            return list;
        }

        /// <summary>
        /// Executes a Non-query operation from a stored procedure. Typically to Insert, Update, or Delete.
        /// </summary>
        /// <param name="storedProcedure"></param>
        /// <param name="sqlParameters"></param>
        public void ExecuteNonQueryStoredProcedure(string storedProcedureName, IEnumerable<SqlParameter> sqlParameters = null)
        {
            try
            {
                using (var cmd = CreateSqlCommand())
                {
                    cmd.CommandText = storedProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (sqlParameters != null)
                    {
                        foreach (var p in sqlParameters)
                        {
                            cmd.Parameters.Add(p);
                        }

                    }

                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();

                }
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>
        /// Executes a Non-query operation from a stored procedure. Typically to Insert, Update, or Delete. Uses a Dictionary for SqlParameters.
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="sqlParametersDictionary"></param>
        public void ExecuteNonQueryStoredProcedure(string storedProcedureName, Dictionary<string, object> sqlParametersDictionary)
        {
            var sqlParameters = sqlParametersDictionary.Select(x => new SqlParameter(x.Key, x.Value));
            ExecuteNonQueryStoredProcedure(storedProcedureName, sqlParameters);
        }

        /// <summary>
        /// Executes a Non-query operation from a stored procedure. Typically to Insert, Update, or Delete. Runs Asynchronously. 
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public async Task ExecuteNonQueryStoredProcedureAsync(string storedProcedureName, IEnumerable<SqlParameter> sqlParameters = null)
        {
            try
            {
                using (var cmd = CreateSqlCommand())
                {
                    cmd.CommandText = storedProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (sqlParameters != null)
                    {
                        foreach (var p in sqlParameters)
                        {
                            cmd.Parameters.Add(p);
                        }
                    }

                    await cmd.Connection.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();

                }
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>
        /// Executes a Non-query operation from a stored procedure. Typically to Insert, Update, or Delete. Runs Asynchronously. Uses a Dictionary for SqlParameters.
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="sqlParametersDictionary"></param>
        /// <returns></returns>
        public async Task ExecuteNonQueryStoredProcedureAsync(string storedProcedureName, Dictionary<string, object> sqlParametersDictionary)
        {
            var sqlParameters = sqlParametersDictionary.Select(x => new SqlParameter(x.Key, x.Value));
            await ExecuteNonQueryStoredProcedureAsync(storedProcedureName, sqlParameters);
        }


        #region HELPER METHODS TO FORMAT EXCEPTION ERRORS.
        private string FormatStoredProcedureError(string storedProcedureName, IEnumerable<SqlParameter> sqlParams, string exceptionMessage)
        {
            var parameters = sqlParams != null ? sqlParams.Select(x => $"[Parameter_Name: {x.ParameterName} - Value: {x.Value}]\n\t").ToList() : null;

            var allParameters = parameters != null ? string.Join("", parameters) : "None";
            var log = $"Stored Procedure Name: {storedProcedureName}.\nStored Procedure Parameters:\n\t{allParameters}";
            var message = $"{exceptionMessage}\n{log}";
            return message;
        }
        #endregion
    }
}
