# Система управления грузовыми перевозками

Учебная практика УП.01.01. WPF-приложение (.NET Framework) с базой данных Microsoft SQL Server.

Тема: проектирование и разработка базы данных для системы управления грузовыми перевозками.

## Структура репозитория

- `README.md` - описание работы и структуры решения
- `db/schema.sql` - скрипт создания базы данных `DB_Cargo` с тестовыми данными
- `res/` - ресурсы приложения (логотип `logo.png`, иконка `logo.ico`)
- `WpfApp1/` - исходный код WPF-проекта

## База данных

Три таблицы, связи и каскадное удаление (`ON DELETE CASCADE`):

| Таблица | Назначение | Поля |
|---|---|---|
| Users | Пользователи системы | ID, Login, Password (хэш SHA-256), Role (User / Admin), FIO, Photo |
| CargoType | Типы грузов | ID, Name |
| Shipment | Грузовые перевозки | ID, CargoTypeID (FK), UserID (FK), ShipDate, Name, Weight, Price |

## Структура проекта

- `Core.cs` - контекст базы данных и текущий пользователь
- `Helper.cs` - хэширование паролей, аутентификация, капча, сообщения, проверки ввода, сохранение в БД, смена темы
- `MainWindow` - главное окно: Frame для страниц, кнопки «Назад», «Выйти из аккаунта», «Сменить тему», дата и время
- `Dictionary.xaml` - словарь стилей по Руководству по стилю (светлая тема)
- `DictionaryDark.xaml` - собственный словарь стилей (тёмная тема)
- `Pages/AuthPage` - авторизация, капча после 3 неверных попыток
- `Pages/RegPage` - регистрация
- `Pages/ChangePassPage` - смена пароля
- `Pages/UserPage` - страница пользователя: просмотр, поиск, сортировка, фильтрация
- `Pages/AdminPage` - страница администратора
- `Pages/UsersTabPage`, `AddUserPage` - таблица пользователей и форма
- `Pages/CargoTypeTabPage`, `AddCargoTypePage` - таблица типов грузов и форма
- `Pages/ShipmentTabPage`, `AddShipmentPage` - таблица перевозок (с поиском, сортировкой, фильтром) и форма
- `Pages/DiagramPage` - диаграмма расходов и экспорт в .xlsx, .docx, .pdf

## Запуск

1. Выполнить `db/schema.sql` в SQL Server Management Studio.
2. Открыть решение в Visual Studio, при необходимости обновить модель `Model.edmx` (Update Model from Database).
3. Для экспорта нужны установленные Microsoft Word и Microsoft Excel.
4. Для шрифта Open Sans установить его в Windows (Google Fonts).

## Тестовые учётные записи

| Логин | Пароль | Роль |
|---|---|---|
| admin | Admin123 | Admin |
| ivanov | User123 | User |
| petrova | User123 | User |
| sidorov | User123 | User |
