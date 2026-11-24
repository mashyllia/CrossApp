using Core;
using Core.Abstractions;
using Core.Events;
using Core.Services;
using Core.Storage;
using Cli.Notifications;
using Core.Import;

Console.OutputEncoding = System.Text.Encoding.UTF8;

bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "orders.json");

IOrderStore store = useFile
    ? new FileOrderStore(dataPath)
    : new InMemoryOrderStore(SampleData.Orders());

var service = new OrderService(store);

// Крок 3. Налаштування логування та перший підписник (локальний метод)
string logPath = Path.Combine(AppContext.BaseDirectory, "logs", "app.log");
Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

void OnOrderChanged(object? sender, OrderChangedEventArgs e)
{
    string line = $"{e.At:HH:mm:ss} {e.Kind,-15} {e.OrderId} | позицій: {e.LinesCount,2} -> сума: {e.Total,10:F2} грн";
    Console.WriteLine(line);
    File.AppendAllText(logPath, line + Environment.NewLine);
}

service.Changed += OnOrderChanged;

// Крок 4. Другий підписник — лямбда із замиканням над лічильником
int changes = 0;
service.Changed += (_, _) => changes++;

// Крок 5. Сценарій змін та відписка
Console.WriteLine("=== Сценарій 1: Робота з подіями та відписка ===");
var created = service.CreateOrder("CUST-77");
service.AddLine(created.Id, "P-001", "Ноутбук Apple MacBook Air", 39999.00m, 1);
service.AddLine(created.Id, "P-002", "Миша бездротова", 1200.50m, 2);

Console.WriteLine($"Подій отримано: {changes}");

// Відписуємо консольно-файловий логер; лямбда-лічильник залишається підписаною
service.Changed -= OnOrderChanged;

service.Confirm(created.Id); // тиха операція для логера, але лічильник зросте
Console.WriteLine($"Подій отримано після відписки логера: {changes}");
Console.WriteLine();

// Крок 6. Сценарій відмови (інваріанти захищають від подій)
Console.WriteLine("=== Сценарій 2: Порушення інваріантів (події не виникають) ===");
try
{
    service.AddLine(created.Id, "P-003", "Монітор", 14500.00m, -5);
}
catch (Exception ex)
{
    Console.WriteLine($"[Очікувана помилка] {ex.GetType().Name}: {ex.Message}");
}

try
{
    service.Confirm("ORD-NON-EXISTENT");
}
catch (Exception ex)
{
    Console.WriteLine($"[Очікувана помилка] {ex.GetType().Name}: {ex.Message}");
}

Console.WriteLine($"Подій після невдалих спроб (має не змінитися): {changes}");
Console.WriteLine();

// Крок 7. Використання Func<Order, bool> замість події для отримання даних
Console.WriteLine("=== Сценарій 3: Пошук через Func<Order, bool> (сума > 5000 грн) ===");
var expensiveOrders = service.FindAll(o => o.Total > 5000m);
foreach (var o in expensiveOrders)
{
    Console.WriteLine($"  {o.Id,-10} | Клієнт: {o.CustomerId,-8} | Статус: {o.Status,-10} | Сума: {o.Total,10:F2} грн");
}

Console.WriteLine();
Console.WriteLine("=== Додаткове завдання 1: INotificationSink через ручний DI ===");
INotificationSink sink = args.Contains("--file-sink")
    ? new FileSink(logPath)
    : new ConsoleSink();

var diService = new OrderService(store, sink);
var diOrder = diService.CreateOrder("CUST-88");
diService.AddLine(diOrder.Id, "P-005", "Кабель USB-C", 450.00m, 3);

Console.WriteLine();
Console.WriteLine("=== Додаткове завдання 2: Action<string> при імпорті CSV ===");
string csvPath = Path.Combine("data", "sample.csv");
if (File.Exists(csvPath))
{
    var importResult = ProductCsvImporter.Load(
        csvPath,
        onSkipped: reason => Console.WriteLine($"  [Пропущено під час імпорту] {reason}")
    );
    Console.WriteLine($"Успішно імпортовано товарів: {importResult.Items.Count}");
}