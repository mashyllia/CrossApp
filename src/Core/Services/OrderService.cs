using Core.Abstractions;
using Core.Domain;
using Core.Events;

namespace Core.Services;

public sealed class OrderService(IOrderStore store)
{
    private readonly IOrderStore _store = store ?? throw new ArgumentNullException(nameof(store));

    public event EventHandler<OrderChangedEventArgs>? Changed;

    private void OnChanged(Order order, OrderChangeKind kind) =>
        Changed?.Invoke(this, new OrderChangedEventArgs(
            order.Id,
            kind,
            order.Lines.Count,
            order.Total));

    public Order CreateOrder(string customerId, string? customId = null)
    {
        string id = string.IsNullOrWhiteSpace(customId)
            ? "ORD-" + Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()
            : customId;

        var order = Order.Create(id, customerId);
        _store.Add(order);
        OnChanged(order, OrderChangeKind.OrderCreated); // після зміни, не до
        return order;
    }

    public void AddLine(string orderId, string productId, string productName, decimal price, int quantity)
    {
        var order = _store.GetById(orderId)
            ?? throw new InvalidOperationException($"Замовлення з id={orderId} не знайдено.");

        order.AddLine(productId, productName, price, quantity);
        _store.Update(order);
        OnChanged(order, OrderChangeKind.LineAdded);
    }

    public void Confirm(string orderId)
    {
        var order = _store.GetById(orderId)
            ?? throw new InvalidOperationException($"Замовлення з id={orderId} не знайдено.");

        order.Confirm();
        _store.Update(order);
        OnChanged(order, OrderChangeKind.OrderConfirmed);
    }

    public IReadOnlyList<Order> All() => _store.List();

    public Order? Find(string id) => _store.GetById(id);

    // Пошук за предикатом (делегат Func замість події)
    public IReadOnlyList<Order> FindAll(Func<Order, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return _store.List().Where(predicate).ToList();
    }
}