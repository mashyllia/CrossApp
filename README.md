# CrossApp

Наскрізний проєкт з крос-платформного програмування.

Предметна область: Замовлення. 
Сутності: Customer (клієнт), Product (товар), Order (замовлення), OrderLine (рядок замовлення).
Призначення: оформлення замовлень і підрахунок сум.

Додаткове 1.1
Розмір publish: osx-arm64 — 83МБ, win-x64 — 77МБ

lab2
## Структура рішення (Solution)

CrossApp/
├── CrossApp.sln
├── README.md
├── .gitignore
└── src/
    ├── Core/
    │   ├── Core.csproj
    │   └── EnvironmentInfo.cs
    └── Cli/
        ├── Cli.csproj
        └── Program.cs

таблиця
RID | Режим | Розмір publish | Потрібен runtime
osx-arm64 | self-contained | ~75-83 МБ | ні
osx-arm64 | framework-dependent | ~180 КБ | так (.NET 10)

 
## Запуск
```bash
dotnet build
dotnet run --project src/Cli
dotnet run --project src/Cli/Cli.csproj
dotnet run --project src/Cli/Cli.csproj -- --json
dotnet publish src/Cli/Cli.csproj -c Release -r osx-arm64 --self-contained true
dotnet publish src/Cli/Cli.csproj -c Release -r osx-arm64 --self-contained false