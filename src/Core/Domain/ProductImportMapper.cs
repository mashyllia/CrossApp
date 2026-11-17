using Core.Dto;

namespace Core.Domain;

public static class ProductImportMapper
{
    public static ImportResult<Product> ToDomain(ImportResult<ProductDto> rawImport)
    {
        var validProducts = new List<Product>();
        var allErrors = new List<string>(rawImport.Errors);

        foreach (var dto in rawImport.Items)
        {
            try
            {
                Product product = Product.FromDto(dto);
                validProducts.Add(product);
            }
            catch (Exception ex)
            {
                allErrors.Add($"Доменна помилка для '{dto.Id}': {ex.Message}");
            }
        }

        return new ImportResult<Product>(validProducts, allErrors);
    }
}