using Core;
using Core.Abstractions;
using Core.Services;
using Core.Storage;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Composition Root: вибір реалізації сховища через аргумент --file
bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "orders.json");

IOrderStore store = useFile
    ? new FileOrderStore(dataPath)
    : new InMemoryOrderStore(SampleData.Orders());

// Ручне впровадження залежності (Manual DI) через конструктор
var service = new OrderService(store);

Console.WriteLine($"Сховище: {store.GetType().Name}");
if (useFile)
    Console.WriteLine($"Файл даних: {dataPath}");
Console.WriteLine(new string('-', 60));

// 1. Основний сценарій: створення замовлення, додавання позицій, підтвердження
var created = service.CreateOrder("CUST-99");
service.AddLine(created.Id, "P-004", "Клавіатура механічна", 3200.00m, 1);
service.AddLine(created.Id, "P-007", "Чохол для ноутбука", 850.00m, 1);
service.Confirm(created.Id);

// 2. Пошук за ID
var found = service.Find(created.Id);
Console.WriteLine($"Щойно створене та підтверджене: {found}");
Console.WriteLine();

// 3. Виведення всіх замовлень у сховищі
Console.WriteLine($"Усього замовлень у сховищі: {service.All().Count}");
foreach (var o in service.All())
{
    Console.WriteLine($"  {o.Id,-10} | Клієнт: {o.CustomerId,-8} | Статус: {o.Status,-10} | Сума: {o.Total,10:F2} грн");
}
Console.WriteLine();

// 4. Сценарій відмови: перехоплення винятків у Cli
Console.WriteLine("=== Перевірка сценарію відмови ===");
try
{
    service.Confirm("ORD-999-UNKNOWN");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"[Очікувана помилка] Операцію відхилено: {ex.Message}");
}

try
{
    service.CreateOrder("CUST-01", created.Id); // спроба створити дубль ID
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"[Очікувана помилка] Дублікат запису: {ex.Message}");
}