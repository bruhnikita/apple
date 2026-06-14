CREATE OR ALTER VIEW dbo.vMaterialList AS
SELECT
    m.*,
    mt.Title AS MaterialTypeTitle,
    CASE
        WHEN ISNULL(m.CountInStock, 0) < m.MinCount THEN '#f19292'
        WHEN ISNULL(m.CountInStock, 0) >= m.MinCount * 3 THEN '#ffba01'
        ELSE '#ffffff'
    END AS HighlightColor,
    CASE
        WHEN m.MinCount <= ISNULL(m.CountInStock, 0) THEN 0
        ELSE CEILING((m.MinCount - ISNULL(m.CountInStock, 0)) / NULLIF(m.CountInPack, 0)) * m.Cost
    END AS RequiredPurchaseCost
FROM dbo.Material m
JOIN dbo.MaterialType mt ON mt.ID = m.MaterialTypeID;
GO
