# Краткая инструкция запуска

В этом репозитории лежит готовое WPF-приложение с SQL Server. Инструкция рассчитана на чистый компьютер в аудитории.

## 1. Клонирование из Git Bash

```bash
mkdir -p /c/ExamDemo
cd /c/ExamDemo
git clone https://github.com/bruhnikita/apple.git
cd apple
```

Если Git попросит логин, используй имя пользователя GitHub `bruhnikita`. В поле пароля нужен GitHub token, а не пароль от аккаунта. Если откроется окно Git Credential Manager, заверши вход в браузере и при необходимости повтори `git clone`.

## 2. Удаление этой подсказки после клонирования

Файл можно удалить из локальной папки экзамена, чтобы он не был виден при демонстрации:

```bash
rm RUNBOOK.md
```

Удаление затронет только локальную папку. На GitHub файл останется.

## 3. Проверка инструментов

```bash
git --version
dotnet --version
```

Для развёртывания базы должен быть запущен SQL Server, а команда `sqlcmd` должна быть доступна из консоли. По умолчанию скрипты используют экземпляр `.\SQLEXPRESS`.

Если в аудитории используется другой экземпляр SQL Server, укажи его перед запуском скрипта:

```bash
export SQLSERVER='localhost'
```

или, например:

```bash
export SQLSERVER='localhost\SQLEXPRESS'
```

## 4. Развёртывание базы данных

Из Git Bash, находясь в папке проекта:

```bash
powershell.exe -ExecutionPolicy Bypass -File "database/sqlserver/setup-db.ps1"
```

Если нужно пересоздать базу с нуля:

```bash
powershell.exe -ExecutionPolicy Bypass -File "database/sqlserver/reset-db.ps1"
```

## 5. Сборка проекта

```bash
dotnet build "src/BolshayaPachkaMaterials.sln"
```

## 6. Проверка логики

```bash
dotnet run --project "src/BolshayaPachkaMaterials.TestRunner/BolshayaPachkaMaterials.TestRunner.csproj"
```

Ожидаемый вывод: `All tests passed`.

## 7. Запуск приложения

```bash
dotnet run --project "src/BolshayaPachkaMaterials.App/BolshayaPachkaMaterials.App.csproj"
```

## 8. Что показать на демонстрации

- Главное окно со списком материалов.
- Записи загружаются из SQL Server сразу после запуска.
- Поиск по верхнему полю.
- Сортировку и фильтр по типу материала.
- Переход по страницам и счётчик записей внизу.
- Добавление и редактирование материала.
- Замену изображения материала.
- Удаление материала и блокировку удаления при зависимости от продукции.
- Массовое изменение минимального остатка.
- Кнопку `Обновить` после изменений в базе.

## 9. Команды с полными путями для папки C:\ExamDemo\apple

```powershell
powershell -ExecutionPolicy Bypass -File "C:\ExamDemo\apple\database\sqlserver\setup-db.ps1"
dotnet build "C:\ExamDemo\apple\src\BolshayaPachkaMaterials.sln"
dotnet run --project "C:\ExamDemo\apple\src\BolshayaPachkaMaterials.TestRunner\BolshayaPachkaMaterials.TestRunner.csproj"
dotnet run --project "C:\ExamDemo\apple\src\BolshayaPachkaMaterials.App\BolshayaPachkaMaterials.App.csproj"
```
