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

lab4
## Інваріанти доменної моделі (Лабораторна робота №4)
* **Order (Замовлення):**
  * Номер замовлення (`Id`) та ідентифікатор клієнта (`CustomerId`) не можуть бути порожніми (`ArgumentException`).
  * Неможливо додати новий рядок до вже підтвердженого замовлення (`InvalidOperationException`).
  * Неможливо підтвердити порожнє замовлення або підтвердити його повторно (`InvalidOperationException`).
  * Колекція рядків `Lines` захищена від зовнішніх змін (`IReadOnlyList<OrderLine>`).
* **OrderLine (Рядок замовлення):**
  * Ідентифікатор (`ProductId`) та назва товару (`ProductName`) обов'язкові (`ArgumentException`).
  * Ціна товару не може бути від'ємною (`ArgumentOutOfRangeException`).
  * Кількість товару в рядку має бути більшою за нуль (`ArgumentOutOfRangeException`).

lab5
## Доменні події, делегати та лямбда-вирази (Лабораторна робота №6)

* **Механізм сповіщень (`Core/Events/`):**
  * `OrderChangeKind` — перелічення типів змін стану замовлення (`OrderCreated`, `LineAdded`, `OrderConfirmed`)[cite: 1, 7].
  * `OrderChangedEventArgs` — аргументи події (`OrderId`, `Kind`, `LinesCount`, `Total`, `At`)[cite: 1, 7].
  * У сервісі `OrderService` оголошено подію `public event EventHandler<OrderChangedEventArgs>? Changed` та приватний метод `OnChanged(...)`, який викликає подію через `Changed?.Invoke(...)` **лише після** успішної зміни стану та збереження у сховищі[cite: 1, 2, 3]. Бібліотека `Core` не містить посилань на `Console` чи файлову систему для логів[cite: 1, 8].
* **Делегати та підписки (`Cli/Program.cs`):**
  * Локальний метод `OnOrderChanged` підписано на `service.Changed` — виводить повідомлення про зміну в консоль та дописує його у файл `logs/app.log` (створюється через `Path.Combine` та `Directory.CreateDirectory`)[cite: 4, 8].
  * Лямбда-вираз із замиканням `(_, _) => changes++` підраховує загальну кількість успішних подій[cite: 4, 6].
  * Продемонстровано відписку від події (`service.Changed -= OnOrderChanged`) та захист від генерації подій при порушенні інваріантів[cite: 4, 7].
* **Пошук через делегат `Func`:**
  * Реалізовано метод `OrderService.FindAll(Func<Order, bool> predicate)` для вибірки замовлень за довільним критерієм (наприклад, `o => o.Total > 5000m`)[cite: 3, 4].
 
## Запуск
```bash
dotnet build
dotnet run --project src/Cli
dotnet run --project src/Cli/Cli.csproj
dotnet run --project src/Cli/Cli.csproj -- --json
dotnet publish src/Cli/Cli.csproj -c Release -r osx-arm64 --self-contained true
dotnet publish src/Cli/Cli.csproj -c Release -r osx-arm64 --self-contained false