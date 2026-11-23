using Core.Domain;

namespace Core;

public static class SampleData
{
    public static IEnumerable<Order> Orders()
    {
        var o1 = Order.Create("ORD-001", "CUST-01");
        o1.AddLine("P-001", "Ноутбук Apple MacBook Air", 39999.00m, 1);
        o1.Confirm();

        var o2 = Order.Create("ORD-002", "CUST-02");
        o2.AddLine("P-002", "Миша бездротова", 1200.50m, 2);

        var o3 = Order.Create("ORD-003", "CUST-03");
        o3.AddLine("P-003", "Монітор 27 дюймів", 14500.00m, 1);
        o3.AddLine("P-005", "Кабель USB-C", 450.00m, 2);
        o3.Confirm();

        return [o1, o2, o3];
    }
}