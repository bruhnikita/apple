# Test cases - materials

| ID | Scenario | Steps | Expected result |
|---|---|---|---|
| TC-01 | Database setup | Run `setup-db.ps1` | SQL Server database is created and seeded |
| TC-02 | Open list | Start the app after DB setup | Material list opens from SQL Server, 15 records per page |
| TC-03 | Search | Enter part of material name or description | Only matching materials remain; counter changes |
| TC-04 | Sort by name | Select name sort | Materials are ordered by title |
| TC-05 | Sort by stock | Select stock sort | Materials are ordered by CountInStock |
| TC-06 | Sort by cost | Select cost sort | Materials are ordered by Cost |
| TC-07 | Filter by type | Select a material type | Only materials of selected type are shown; All types resets filter |
| TC-08 | Combined search/sort/filter | Apply filter, then search, then sort | All operations work together |
| TC-09 | Highlight low stock | Find item where CountInStock < MinCount | Card background is #f19292 |
| TC-10 | Highlight high stock | Find item where CountInStock >= MinCount * 3 | Card background is #ffba01 |
| TC-11 | Mass min count update | Select rows, enter value, click update | Selected MinCount values are updated in SQL Server and history rows are added |
| TC-12 | Add/edit material | Use Add or Edit and save | Material row and supplier links are saved in SQL Server |
| TC-13 | Replace image | Use image replacement in edit form | Selected file is copied to resources/images and relative path is saved |
| TC-14 | Delete rule | Delete material with ProductMaterial dependency | Deletion is blocked; materials without product dependency delete related suppliers/history first |
