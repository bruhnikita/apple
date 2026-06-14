using System.Data;
using System.Data.SqlClient;
using Exam.Core;

namespace Exam.Data;

public sealed class MaterialRepository
{
    private readonly string connectionString;

    public MaterialRepository(string? connectionString = null)
    {
        this.connectionString = connectionString ?? DatabaseSettings.BuildConnectionString("BolshayaPachkaMaterials");
    }

    public List<Material> Load()
    {
        const string sql = """
            SELECT
                m.ID,
                m.Title,
                m.MaterialTypeID,
                mt.Title AS MaterialTypeTitle,
                ISNULL(m.Description, N'') AS Description,
                ISNULL(m.Image, N'picture.png') AS Image,
                m.Cost,
                ISNULL(m.CountInStock, 0) AS CountInStock,
                m.MinCount,
                m.CountInPack,
                m.Unit,
                ISNULL(STUFF((
                    SELECT N', ' + s.Title
                    FROM dbo.MaterialSupplier ms
                    JOIN dbo.Supplier s ON s.ID = ms.SupplierID
                    WHERE ms.MaterialID = m.ID
                    ORDER BY s.Title
                    FOR XML PATH(''), TYPE
                ).value('.', 'nvarchar(max)'), 1, 2, N''), N'') AS Suppliers,
                CASE WHEN EXISTS (SELECT 1 FROM dbo.ProductMaterial pm WHERE pm.MaterialID = m.ID) THEN 1 ELSE 0 END AS HasProductMaterial
            FROM dbo.Material m
            JOIN dbo.MaterialType mt ON mt.ID = m.MaterialTypeID
            ORDER BY m.Title;
            """;

        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand(sql, connection);
        connection.Open();
        using var reader = command.ExecuteReader();

        var items = new List<Material>();
        while (reader.Read())
        {
            items.Add(new Material
            {
                Id = reader.GetInt32("ID"),
                Title = reader.GetString("Title"),
                MaterialTypeId = reader.GetInt32("MaterialTypeID"),
                Type = reader.GetString("MaterialTypeTitle"),
                Description = reader.GetString("Description"),
                Image = reader.GetString("Image"),
                Cost = reader.GetDecimal("Cost"),
                CountInStock = Convert.ToDouble(reader["CountInStock"]),
                MinCount = Convert.ToDouble(reader["MinCount"]),
                CountInPack = reader.GetInt32("CountInPack"),
                Unit = reader.GetString("Unit"),
                Suppliers = reader.GetString("Suppliers"),
                HasProductMaterial = Convert.ToInt32(reader["HasProductMaterial"]) == 1
            });
        }
        return items;
    }

    public List<LookupItem> GetTypes()
    {
        const string sql = "SELECT ID, Title FROM dbo.MaterialType ORDER BY Title;";
        return LoadLookup(sql);
    }

    public List<string> GetSupplierTitles()
    {
        const string sql = "SELECT Title FROM dbo.Supplier ORDER BY Title;";
        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand(sql, connection);
        connection.Open();
        using var reader = command.ExecuteReader();
        var result = new List<string>();
        while (reader.Read()) result.Add(reader.GetString(0));
        return result;
    }

    public void Save(Material material)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            if (material.Id == 0)
                Insert(connection, transaction, material);
            else
                Update(connection, transaction, material);

            ReplaceSuppliers(connection, transaction, material.Id, material.Suppliers);
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void BulkUpdateMinCount(IEnumerable<int> materialIds, double minCount)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            foreach (var id in materialIds.Distinct())
            {
                Execute(connection, transaction,
                    "UPDATE dbo.Material SET MinCount=@MinCount WHERE ID=@ID;",
                    new SqlParameter("@MinCount", minCount), new SqlParameter("@ID", id));
                Execute(connection, transaction,
                    "INSERT INTO dbo.MaterialCountHistory(MaterialID, ChangeDate, CountValue) VALUES(@ID, GETDATE(), @MinCount);",
                    new SqlParameter("@ID", id), new SqlParameter("@MinCount", minCount));
            }
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public DeleteResult Delete(IEnumerable<Material> materials)
    {
        var result = new DeleteResult();
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            foreach (var material in materials)
            {
                var hasProducts = Convert.ToInt32(Scalar(connection, transaction,
                    "SELECT COUNT(*) FROM dbo.ProductMaterial WHERE MaterialID=@ID;",
                    new SqlParameter("@ID", material.Id))) > 0;
                if (hasProducts)
                {
                    result.Blocked++;
                    result.Messages.Add($"Удаление материала '{material.Title}' запрещено: он используется в продукции.");
                    continue;
                }

                Execute(connection, transaction, "DELETE FROM dbo.MaterialSupplier WHERE MaterialID=@ID;", new SqlParameter("@ID", material.Id));
                Execute(connection, transaction, "DELETE FROM dbo.MaterialCountHistory WHERE MaterialID=@ID;", new SqlParameter("@ID", material.Id));
                Execute(connection, transaction, "DELETE FROM dbo.Material WHERE ID=@ID;", new SqlParameter("@ID", material.Id));
                result.Deleted++;
            }
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
        return result;
    }

    private void Insert(SqlConnection connection, SqlTransaction transaction, Material material)
    {
        const string sql = """
            INSERT INTO dbo.Material(Title, CountInPack, Unit, CountInStock, MinCount, Description, Cost, Image, MaterialTypeID)
            OUTPUT INSERTED.ID
            VALUES(@Title, @CountInPack, @Unit, @CountInStock, @MinCount, @Description, @Cost, @Image, @MaterialTypeID);
            """;
        material.Id = Convert.ToInt32(Scalar(connection, transaction, sql,
            new SqlParameter("@Title", material.Title),
            new SqlParameter("@CountInPack", material.CountInPack),
            new SqlParameter("@Unit", material.Unit),
            new SqlParameter("@CountInStock", material.CountInStock),
            new SqlParameter("@MinCount", material.MinCount),
            new SqlParameter("@Description", material.Description),
            new SqlParameter("@Cost", material.Cost),
            new SqlParameter("@Image", string.IsNullOrWhiteSpace(material.Image) ? "picture.png" : material.Image),
            new SqlParameter("@MaterialTypeID", material.MaterialTypeId)));
    }

    private void Update(SqlConnection connection, SqlTransaction transaction, Material material)
    {
        const string sql = """
            UPDATE dbo.Material
            SET Title=@Title,
                CountInPack=@CountInPack,
                Unit=@Unit,
                CountInStock=@CountInStock,
                MinCount=@MinCount,
                Description=@Description,
                Cost=@Cost,
                Image=@Image,
                MaterialTypeID=@MaterialTypeID
            WHERE ID=@ID;
            """;
        Execute(connection, transaction, sql,
            new SqlParameter("@ID", material.Id),
            new SqlParameter("@Title", material.Title),
            new SqlParameter("@CountInPack", material.CountInPack),
            new SqlParameter("@Unit", material.Unit),
            new SqlParameter("@CountInStock", material.CountInStock),
            new SqlParameter("@MinCount", material.MinCount),
            new SqlParameter("@Description", material.Description),
            new SqlParameter("@Cost", material.Cost),
            new SqlParameter("@Image", string.IsNullOrWhiteSpace(material.Image) ? "picture.png" : material.Image),
            new SqlParameter("@MaterialTypeID", material.MaterialTypeId));
    }

    private void ReplaceSuppliers(SqlConnection connection, SqlTransaction transaction, int materialId, string suppliersText)
    {
        Execute(connection, transaction, "DELETE FROM dbo.MaterialSupplier WHERE MaterialID=@ID;", new SqlParameter("@ID", materialId));
        foreach (var title in SplitSuppliers(suppliersText))
        {
            var supplierId = Convert.ToInt32(Scalar(connection, transaction,
                """
                IF NOT EXISTS (SELECT 1 FROM dbo.Supplier WHERE Title=@Title)
                    INSERT INTO dbo.Supplier(Title, INN, StartDate, QualityRating, SupplierType)
                    VALUES(@Title, RIGHT('0000000000' + CAST(ABS(CHECKSUM(@Title)) AS varchar(10)), 10), CAST(GETDATE() AS date), 10, N'local');
                SELECT ID FROM dbo.Supplier WHERE Title=@Title;
                """,
                new SqlParameter("@Title", title)));

            Execute(connection, transaction,
                "INSERT INTO dbo.MaterialSupplier(MaterialID, SupplierID) VALUES(@MaterialID, @SupplierID);",
                new SqlParameter("@MaterialID", materialId), new SqlParameter("@SupplierID", supplierId));
        }
    }

    private static IEnumerable<string> SplitSuppliers(string value) =>
        value.Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase);

    private List<LookupItem> LoadLookup(string sql)
    {
        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand(sql, connection);
        connection.Open();
        using var reader = command.ExecuteReader();
        var result = new List<LookupItem>();
        while (reader.Read())
            result.Add(new LookupItem { Id = reader.GetInt32(0), Title = reader.GetString(1) });
        return result;
    }

    private static void Execute(SqlConnection connection, SqlTransaction transaction, string sql, params SqlParameter[] parameters)
    {
        using var command = new SqlCommand(sql, connection, transaction);
        command.Parameters.AddRange(parameters);
        command.ExecuteNonQuery();
    }

    private static object Scalar(SqlConnection connection, SqlTransaction transaction, string sql, params SqlParameter[] parameters)
    {
        using var command = new SqlCommand(sql, connection, transaction);
        command.Parameters.AddRange(parameters);
        return command.ExecuteScalar()!;
    }
}

