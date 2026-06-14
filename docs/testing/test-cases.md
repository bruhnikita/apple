# Test cases - materials

| ID | Scenario | Steps | Expected result |
|---|---|---|---|
| TC-01 | Open list | Start the app | Material list opens, real imported records are visible, 15 records per page |
| TC-02 | Search | Enter part of material name or description | Only matching materials remain; counter changes |
| TC-03 | Sort by name | Select name sort | Materials are ordered by title |
| TC-04 | Sort by stock | Select stock sort | Materials are ordered by CountInStock |
| TC-05 | Sort by cost | Select cost sort | Materials are ordered by Cost |
| TC-06 | Filter by type | Select a material type | Only materials of selected type are shown; All types resets filter |
| TC-07 | Combined search/sort/filter | Apply filter, then search, then sort | All operations work together |
| TC-08 | Highlight low stock | Find item where CountInStock < MinCount | Card background is #f19292 |
| TC-09 | Highlight high stock | Find item where CountInStock >= MinCount * 3 | Card background is #ffba01 |
| TC-10 | Mass min count update | Select rows, enter value, click update | Selected MinCount values change |
| TC-11 | Add/edit/delete | Use toolbar buttons | Record is added, edited, or deletion is blocked when product dependency exists |
| TC-12 | Reset data | Click reset | Initial imported demo records return |
