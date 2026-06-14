using System.Data.SqlClient;

namespace Exam.Data;

internal static class SqlDataReaderExtensions
{
    public static int GetInt32(this SqlDataReader reader, string column) => reader.GetInt32(reader.GetOrdinal(column));
    public static string GetString(this SqlDataReader reader, string column) => reader.GetString(reader.GetOrdinal(column));
    public static decimal GetDecimal(this SqlDataReader reader, string column) => reader.GetDecimal(reader.GetOrdinal(column));
}
