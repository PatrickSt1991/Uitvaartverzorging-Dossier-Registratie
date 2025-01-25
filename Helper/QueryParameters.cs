using System;
using System.Data;
using System.Data.SqlClient;

namespace Dossier_Registratie.Helper
{
    public static class QueryParameters
    {
        //Insert Statements
        public static void AddDbNull(SqlCommand command, string parameterName, object value)
        {
            command.Parameters.AddWithValue(parameterName, value ?? DBNull.Value);
        }
        public static void AddStringEmpty(SqlCommand command, string parameterName, object value)
        {
            command.Parameters.AddWithValue(parameterName, value ?? string.Empty);
        }
        public static void AddDateTime(SqlCommand command, string parameterName, DateTime? value)
        {
            command.Parameters.Add(new SqlParameter(parameterName, SqlDbType.DateTime)
            {
                Value = value.HasValue ? value.Value : DBNull.Value
            });
        }
        public static void AddBoolean(SqlCommand command, string parameterName, bool? value)
        {
            command.Parameters.AddWithValue(parameterName, value.HasValue ? (object)value.Value : DBNull.Value);
        }
    }
}
