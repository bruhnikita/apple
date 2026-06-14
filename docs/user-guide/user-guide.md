# User guide - materials

1. Run `database/sqlserver/setup-db.ps1` to create and seed SQL Server database.
2. Start the app from `src/BolshayaPachkaMaterials.App`.
3. Use the top search field to search by material name and description.
4. Use sorting to order by name, stock, or cost.
5. Use filtering to select one material type; `Все типы` shows all records.
6. The bottom counter shows how many records are displayed from the filtered total.
7. Use page buttons to move through the list; each page shows 15 records.
8. Low stock materials are highlighted red; high stock materials are highlighted yellow.
9. Select records and use the mass update panel to change minimum count in SQL Server.
10. Add/Edit opens a form with material fields, suppliers and image replacement.
11. Delete removes supplier/history dependencies and blocks deletion when the material is used by a product.
12. Missing images use `resources/picture.png`.
13. Use `Обновить` to reload records from SQL Server after external database changes.
