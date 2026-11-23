using Cli;
using Core.Abstractions;
using Core.Domain;
using Core.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Composition Root через StoreFactory (підтримує --file та --cache)
IOrderStore store = StoreFactory.Create(args);
var service = new OrderService(store);

Console.WriteLine($"Сховище: {store.GetType().Name}");
Console.WriteLine(new string('-', 60));

// 1. Основний сценарій: створення, додавання позицій, підтвердження
var created = service.CreateOrder("CUST-99");
service.AddLine(created.Id, "P-004", "Клавіатура механічна", 3200.00m, 1);
service.AddLine(created.Id, "P-007", "Чохол для ноутбука", 850.00m, 1);
service.Confirm(created.Id);

var found = service.Find(created.Id);
Console.WriteLine($"Щойно створене та підтверджене: {found}");
Console.WriteLine();

// 2. Виведення всіх замовлень
Console.WriteLine($"Усього замовлень у сховищі: {service.All().Count}");
foreach (var o in service.All())
{
    Console.WriteLine($"  {o.Id,-10} | Клієнт: {o.CustomerId,-8} | Статус: {o.Status,-10} | Сума: {o.Total,10:F2} грн");
}
Console.WriteLine();

// 3. Додаткове завдання 2: пошук за предикатом Func<Order, bool>
var confirmedExpensiveOrders = service.FindWhere(o => o.Status == OrderStatus.Confirmed && o.Total > 4000m);
Console.WriteLine($"Підтверджені замовлення дорожче 4000 грн ({confirmedExpensiveOrders.Count}):");
foreach (var o in confirmedExpensiveOrders)
{
    Console.WriteLine($"  * {o.Id}: {o.Total:F2} грн");
}
Console.WriteLine();

// 4. Сценарій відмови
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
    service.CreateOrder("CUST-01", created.Id);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"[Очікувана помилка] Дублікат запису: {ex.Message}");
}