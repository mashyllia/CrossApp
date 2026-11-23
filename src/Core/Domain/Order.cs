using Core.Dto;

namespace Core.Domain;

public sealed class Order
{
    private readonly List<OrderLine> _lines = [];

    public string Id { get; }
    public string CustomerId { get; }
    public OrderStatus Status { get; private set; } 
    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();
    public decimal Total => _lines.Sum(line => line.LineTotal);

    private Order(string id, string customerId, OrderStatus status = OrderStatus.Draft)
    {
        Id = id;
        CustomerId = customerId;
        Status = status;
    }

    public static Order Create(string id, string customerId)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Номер замовлення обов'язковий", nameof(id));

        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("Ідентифікатор клієнта обов'язковий", nameof(customerId));

        return new Order(id.Trim().ToUpperInvariant(), customerId.Trim());
    }

    public void AddLine(string productId, string productName, decimal price, int quantity)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException(
                $"Не можна додавати товари до замовлення {Id} у статусі {Status}");

        OrderLine newLine = OrderLine.Create(productId, productName, price, quantity);
        _lines.Add(newLine);
    }

    public void Confirm()
    {
        EnsureTransition(OrderStatus.Confirmed);

        if (_lines.Count == 0)
            throw new InvalidOperationException($"Не можна підтвердити порожнє замовлення {Id}");

        Status = OrderStatus.Confirmed;
    }

    public void Cancel()
    {
        EnsureTransition(OrderStatus.Cancelled);
        Status = OrderStatus.Cancelled;
    }

    // Перевірка допустимих переходів стану через switch expression
    private void EnsureTransition(OrderStatus next)
    {
        bool isAllowed = (Status, next) switch
        {
            (OrderStatus.Draft, OrderStatus.Confirmed) => true,
            (OrderStatus.Draft, OrderStatus.Cancelled) => true,
            (OrderStatus.Confirmed, OrderStatus.Cancelled) => true,
            _ => false
        };

        if (!isAllowed)
            throw new InvalidOperationException(
                $"Перехід замовлення {Id} зі статусу '{Status}' у '{next}' заборонений");
    }

    public OrderDto ToDto() => new(
        Id,
        CustomerId,
        Status.ToString(),
        _lines.Select(l => l.ToDto()).ToList()
    );

    public static Order FromDto(OrderDto dto)
    {
        Order order = Create(dto.Id, dto.CustomerId);
        foreach (var lineDto in dto.Lines)
        {
            order.AddLine(lineDto.ProductId, lineDto.ProductName, lineDto.Price, lineDto.Quantity);
        }

        if (!Enum.TryParse<OrderStatus>(dto.Status, ignoreCase: true, out var targetStatus))
            throw new ArgumentException($"Невідомий статус замовлення: '{dto.Status}'");

        if (targetStatus == OrderStatus.Confirmed)
            order.Confirm();
        else if (targetStatus == OrderStatus.Cancelled)
            order.Cancel();

        return order;
    }

    public override string ToString() =>
        $"Замовлення {Id} (Клієнт: {CustomerId}) | Статус: {Status} | Позицій: {_lines.Count} | Сума: {Total:F2} грн";
}