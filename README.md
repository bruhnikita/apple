# Apple

Готовое WPF-приложение с SQL Server для учёта материалов. Приложение работает с базой через ADO.NET-репозитории, а база данных создаётся и заполняется скриптами из папки `database/sqlserver`.

## Быстрый запуск

```powershell
powershell -ExecutionPolicy Bypass -File "database\sqlserver\setup-db.ps1"
dotnet build "src\BolshayaPachkaMaterials.sln"
dotnet run --project "src\BolshayaPachkaMaterials.TestRunner\BolshayaPachkaMaterials.TestRunner.csproj"
dotnet run --project "src\BolshayaPachkaMaterials.App\BolshayaPachkaMaterials.App.csproj"
```

По умолчанию используется экземпляр SQL Server `.\SQLEXPRESS`. Для другого экземпляра задай переменную `SQLSERVER` или полную строку подключения `EXAM_CONNECTION_STRING`.

## Что есть в проекте

- Просмотр, поиск, сортировка, фильтрация и постраничный вывод материалов.
- Добавление, редактирование, удаление и замена изображения материала.
- Массовое изменение минимального остатка выбранных материалов.
- Подсветка недостаточного и избыточного остатка.
- Расчёт стоимости закупки до минимального остатка.
- Работа с поставщиками, историей остатков и зависимостями от продукции.
- Документация, тест-кейсы, ERD и диаграмма вариантов использования в папке `docs`.

