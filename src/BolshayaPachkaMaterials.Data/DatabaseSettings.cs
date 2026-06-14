namespace Exam.Data;

public static class DatabaseSettings
{
    public static string BuildConnectionString(string databaseName)
    {
        var explicitConnection = Environment.GetEnvironmentVariable("EXAM_CONNECTION_STRING")
            ?? Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION");
        if (!string.IsNullOrWhiteSpace(explicitConnection))
            return explicitConnection;

        var server = Environment.GetEnvironmentVariable("SQLSERVER");
        if (string.IsNullOrWhiteSpace(server))
            server = @".\SQLEXPRESS";

        return $"Server={server};Database={databaseName};Trusted_Connection=True;TrustServerCertificate=True;";
    }
}
