using Core.Dto;

namespace Core.Domain;

public sealed class Order
{
    private readonly List<OrderLine> _lines = [];

    public string Id { get; }
    public string CustomerId { get; }
    public bool IsConfirmed { get; private set; }
    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();
    public decimal Total => _lines.Sum(line => line.LineTotal);

    private Order(string id, string customerId, bool isConfirmed = false)
    {
        Id = id;
        CustomerId = customerId;
        IsConfirmed = isConfirmed;
    }

    // Фабричний метод створення нового замовлення
    public static Order Create(string id, string customerId)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Номер замовлення обов'язковий", nameof(id));

        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("Ідентифікатор клієнта обов'язковий", nameof(customerId));

        return new Order(id.Trim().ToUpperInvariant(), customerId.Trim());
    }

    // Метод зміни стану 1: додавання рядка до замовлення
    public void AddLine(string productId, string productName, decimal price, int quantity)
    {
        if (IsConfirmed)
            throw new InvalidOperationException(
                $"Замовлення {Id} вже підтверджене, додавати нові товари не можна");

        // Делегуємо перевірку параметрів рядка фабриці OrderLine
        OrderLine newLine = OrderLine.Create(productId, productName, price, quantity);
        _lines.Add(newLine);
    }

    // Метод зміни стану 2: підтвердження замовлення
    public void Confirm()
    {
        if (IsConfirmed)
            throw new InvalidOperationException($"Замовлення {Id} вже підтверджене");

        if (_lines.Count == 0)
            throw new InvalidOperationException($"Не можна підтвердити порожнє замовлення {Id}");

        IsConfirmed = true;
    }

    // Мапінг у DTO (для збереження у сховищі на тижні 5)
    public OrderDto ToDto() => new(
        Id,
        CustomerId,
        IsConfirmed,
        _lines.Select(l => l.ToDto()).ToList()
    );

    // Відновлення з DTO через фабрику (проходить ті самі перевірки!)
    public static Order FromDto(OrderDto dto)
    {
        Order order = Create(dto.Id, dto.CustomerId);
        foreach (var lineDto in dto.Lines)
        {
            order.AddLine(lineDto.ProductId, lineDto.ProductName, lineDto.Price, lineDto.Quantity);
        }
        if (dto.IsConfirmed)
        {
            order.Confirm();
        }
        return order;
    }

    public override string ToString()
    {
        string status = IsConfirmed ? "Підтверджено" : "Чернетка";
        return $"Замовлення {Id} (Клієнт: {CustomerId}) | Статус: {status} | Позицій: {_lines.Count} | Сума: {Total:F2} грн";
    }
}