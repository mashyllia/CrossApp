using System.Text.Json;
using Core.Dto;

namespace Core.Import;

public static class ProductJsonImporter
{
    public static ImportResult<ProductDto> Load(string path)
    {
        var errors = new List<string>();
        try
        {
            string json = File.ReadAllText(path);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var rawItems = JsonSerializer.Deserialize<List<ProductDto>>(json, options) ?? [];

            var items = new List<ProductDto>();
            for (int i = 0; i < rawItems.Count; i++)
            {
                var item = rawItems[i];
                if (string.IsNullOrWhiteSpace(item.Id) || string.IsNullOrWhiteSpace(item.Name))
                {
                    errors.Add($"запис #{i + 1}: ID або назва порожні");
                }
                else if (item.Price < 0)
                {
                    errors.Add($"запис #{i + 1}: ціна '{item.Price}' не може бути від'ємною");
                }
                else
                {
                    items.Add(item);
                }
            }

            return new ImportResult<ProductDto>(items, errors);
        }
        catch (Exception ex)
        {
            errors.Add($"Помилка читання JSON: {ex.Message}");
            return new ImportResult<ProductDto>([], errors);
        }
    }
}