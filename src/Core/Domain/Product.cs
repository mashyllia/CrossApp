using Core.Dto;

namespace Core.Domain;

public sealed class Product
{
    public const decimal MinCatalogPrice = 10.00m;

    public string Id { get; }
    public string Name { get; }
    public decimal Price { get; }

    private Product(string id, string name, decimal price)
    {
        Id = id;
        Name = name;
        Price = price;
    }

    public static Product Create(string id, string name, decimal price)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID товару обов'язковий", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Назва товару обов'язкова", nameof(name));

        if (price < MinCatalogPrice)
            throw new ArgumentOutOfRangeException(nameof(price), price,
                $"Ціна товару '{name}' ({price} грн) менша за мінімально допустиму ({MinCatalogPrice} грн)");

        return new Product(id.Trim().ToUpperInvariant(), name.Trim(), price);
    }

    public ProductDto ToDto() => new(Id, Name, Price);
    public static Product FromDto(ProductDto dto) => Create(dto.Id, dto.Name, dto.Price);

    public override string ToString() => $"{Id}: {Name} — {Price:F2} грн";
}