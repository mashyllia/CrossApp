namespace Core.Domain;

public sealed class Customer
{
    public const int MaxActiveOrders = 2;
    private readonly List<Order> _orders = [];

    public string Id { get; }
    public string FullName { get; }
    public IReadOnlyList<Order> Orders => _orders.AsReadOnly();

    private Customer(string id, string fullName)
    {
        Id = id;
        FullName = fullName;
    }

    public static Customer Create(string id, string fullName)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID клієнта обов'язковий", nameof(id));
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Ім'я клієнта обов'язкове", nameof(fullName));

        return new Customer(id.Trim().ToUpperInvariant(), fullName.Trim());
    }

    // Інваріант, що охоплює дві сутності (Customer + Order)
    public void PlaceOrder(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        if (order.CustomerId != Id)
            throw new InvalidOperationException(
                $"Замовлення {order.Id} належить іншому клієнту ({order.CustomerId})");

        int activeCount = _orders.Count(o => o.Status != OrderStatus.Cancelled);
        if (activeCount >= MaxActiveOrders)
            throw new InvalidOperationException(
                $"Клієнт {FullName} ({Id}) вже має максимум активних замовлень ({MaxActiveOrders})");

        _orders.Add(order);
    }
}