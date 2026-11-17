using Core.Domain;
using Core.Dto;
using Core.Import;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== Сценарій 1: Успішна робота з доменом (OrderStatus) ===");
Order order = Order.Create("ord-101", "CUST-01");
order.AddLine("P-001", "Ноутбук Apple MacBook Air", 39999.00m, 1);
order.AddLine("P-002", "Миша бездротова", 1200.50m, 2);
order.Confirm();
Console.WriteLine(order);

OrderDto dto = order.ToDto();
Order restoredOrder = Order.FromDto(dto);
Console.WriteLine("Відновлено з DTO: " + restoredOrder);
Console.WriteLine();

Console.WriteLine("=== Сценарій 2: Порушення інваріантів та переходів стану ===");
TryDo("Порожній ID замовлення", () => Order.Create("", "CUST-01"));
TryDo("Кількість товару <= 0", () => Order.Create("ORD-102", "CUST-02").AddLine("P-003", "Монітор", 14500m, 0));
TryDo("Підтвердження порожнього замовлення", () => Order.Create("ORD-103", "CUST-03").Confirm());
TryDo("Додавання товару у підтверджене замовлення", () => order.AddLine("P-004", "Клавіатура", 3200m, 1));

// Додаткове завдання 3: переходи статусів (Cancelled -> Confirmed заборонено)
order.Cancel();
Console.WriteLine("Після скасування: " + order);
TryDo("Підтвердження скасованого замовлення", () => order.Confirm());
Console.WriteLine();

Console.WriteLine("=== Додаткове завдання 2: Інваріант між двома сутностями (Customer + Order) ===");
Customer customer = Customer.Create("CUST-01", "Пинчук Марія");
customer.PlaceOrder(Order.Create("ORD-201", "CUST-01"));
customer.PlaceOrder(Order.Create("ORD-202", "CUST-01"));
TryDo("3-тє активне замовлення для одного клієнта", () =>
    customer.PlaceOrder(Order.Create("ORD-203", "CUST-01")));
Console.WriteLine();

Console.WriteLine("=== Додаткове завдання 1: Зв'язок з тижнем 3 (ImportResult -> Domain) ===");
var rawImport = new ImportResult<ProductDto>(
    [
        new ProductDto("P-001", "Ноутбук", 39999m),
        new ProductDto("P-009", "Дешевий стікер", 5m) // порушує MinCatalogPrice (10 грн)
    ],
    ["рядок 12: очікую 3 колонки, отримав 2"]
);
ImportResult<Product> domainImport = ProductImportMapper.ToDomain(rawImport);
Console.WriteLine($"Валідних доменних товарів: {domainImport.Items.Count}");
foreach (var p in domainImport.Items) Console.WriteLine($"  + {p}");
Console.WriteLine($"Усього помилок (парсинг + домен): {domainImport.Errors.Count}");
foreach (var err in domainImport.Errors) Console.WriteLine($"  ! {err}");

static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($"[ПОМИЛКА] {title}: виняток НЕ спрацював!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ОК] {title} -> {ex.GetType().Name}: {ex.Message}");
    }
}