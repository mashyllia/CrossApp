using Core.Domain;
using Core.Dto;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== Сценарій 1: Успішна робота з доменом ===");
Order order = Order.Create("ord-101", "CUST-01");
Console.WriteLine("Створено: " + order);

order.AddLine("P-001", "Ноутбук Apple MacBook Air", 39999.00m, 1);
order.AddLine("P-002", "Миша бездротова", 1200.50m, 2);
Console.WriteLine("Після додавання товарів: " + order);
foreach (var line in order.Lines)
{
    Console.WriteLine($"   - {line}");
}

order.Confirm();
Console.WriteLine("Після підтвердження: " + order);

// Перевірка мапінгу ToDto / FromDto
OrderDto dto = order.ToDto();
Order restoredOrder = Order.FromDto(dto);
Console.WriteLine("Відновлено з DTO: " + restoredOrder);
Console.WriteLine();

Console.WriteLine("=== Сценарій 2: Порушення інваріантів ===");

// 1. Помилки вхідних аргументів (ArgumentException / ArgumentOutOfRangeException)
TryDo("Порожній ID замовлення", () => Order.Create("", "CUST-01"));
TryDo("Кількість товару <= 0", () => 
{
    var tempOrder = Order.Create("ORD-102", "CUST-02");
    tempOrder.AddLine("P-003", "Монітор", 14500m, 0);
});
TryDo("Від'ємна ціна товару", () => 
{
    var tempOrder = Order.Create("ORD-102", "CUST-02");
    tempOrder.AddLine("P-003", "Монітор", -500m, 1);
});

// 2. Помилки стану об'єкта (InvalidOperationException)
TryDo("Підтвердження порожнього замовлення", () => 
{
    var emptyOrder = Order.Create("ORD-103", "CUST-03");
    emptyOrder.Confirm();
});
TryDo("Додавання товару у вже підтверджене замовлення", () => 
{
    order.AddLine("P-004", "Клавіатура", 3200m, 1);
});
TryDo("Повторне підтвердження замовлення", () => 
{
    order.Confirm();
});

Console.WriteLine();
Console.WriteLine("Стан першого замовлення після всіх спроб порушення:");
Console.WriteLine(order);

// Допоміжна функція для перехоплення винятків
static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($"[ПОМИЛКА] {title}: виняток НЕ спрацював — інваріант відсутній!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ОК] {title} -> {ex.GetType().Name}: {ex.Message}");
    }
}