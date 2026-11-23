using Core.Domain;

namespace Core.Abstractions;

public interface IOrderStore
{
    IReadOnlyList<Order> List();
    Order? GetById(string id);
    IReadOnlyList<Order> FindWhere(Func<Order, bool> predicate); // нове
    void Add(Order item);
    void Update(Order item);
    bool Remove(string id);
}