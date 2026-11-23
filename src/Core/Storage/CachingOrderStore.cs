using Core.Abstractions;
using Core.Domain;

namespace Core.Storage;

public sealed class CachingOrderStore(IOrderStore inner) : IOrderStore
{
    private readonly IOrderStore _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    private IReadOnlyList<Order>? _listCache;
    private readonly Dictionary<string, Order?> _byIdCache = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<Order> List()
    {
        return _listCache ??= _inner.List();
    }

    public Order? GetById(string id)
    {
        if (_byIdCache.TryGetValue(id, out var cached))
            return cached;

        var order = _inner.GetById(id);
        _byIdCache[id] = order;
        return order;
    }

    public IReadOnlyList<Order> FindWhere(Func<Order, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return List().Where(predicate).ToList();
    }

    public void Add(Order item)
    {
        _inner.Add(item);
        Invalidate();
    }

    public void Update(Order item)
    {
        _inner.Update(item);
        Invalidate();
    }

    public bool Remove(string id)
    {
        bool removed = _inner.Remove(id);
        if (removed) Invalidate();
        return removed;
    }

    private void Invalidate()
    {
        _listCache = null;
        _byIdCache.Clear();
    }
}