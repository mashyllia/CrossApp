using Core.Abstractions;
using Core.Domain;

namespace Core.Services;

public sealed class OrderService(IOrderStore store)
{
    private readonly IOrderStore _store = store ?? throw new ArgumentNullException(nameof(store));

    public Order CreateOrder(string customerId, string? customId = null)
    {
        string id = string.IsNullOrWhiteSpace(customId)
            ? "ORD-" + Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()
            : customId;

        var order = Order.Create(id, customerId);
        _store.Add(order);
        return order;
    }

    public void AddLine(string orderId, string productId, string productName, decimal price, int quantity)
    {
        var order = _store.GetById(orderId)
            ?? throw new InvalidOperationException($"Замовлення з id={orderId} не знайдено.");

        order.AddLine(productId, productName, price, quantity);
        _store.Update(order);
    }

    public void Confirm(string orderId)
    {
        var order = _store.GetById(orderId)
            ?? throw new InvalidOperationException($"Замовлення з id={orderId} не знайдено.");

        order.Confirm();
        _store.Update(order);
    }

    public IReadOnlyList<Order> All() => _store.List();

    public Order? Find(string id) => _store.GetById(id);
}