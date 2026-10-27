using Core.Dto;
using Core.Import;

Console.OutputEncoding = System.Text.Encoding.UTF8;

string path = args.Length > 0 ? args[0] : Path.Combine("data", "sample.csv");

if (!File.Exists(path))
{
    Console.WriteLine($"Файл не знайдено: {Path.GetFullPath(path)}");
    return 1;
}

// Вибір імпортера за розширенням файлу (.csv /.json) через switch expression
ImportResult<ProductDto> result = Path.GetExtension(path).ToLowerInvariant() switch
{
    ".csv" => ProductCsvImporter.Load(path),
    ".json" => ProductJsonImporter.Load(path),
    var ext => throw new NotSupportedException($"Формат файлу '{ext}' не підтримується")
};

Console.WriteLine($"Завантажено записів: {result.Items.Count}");
foreach (ProductDto p in result.Items.Take(5))
{
    Console.WriteLine($"  {p.Id,-6} {p.Name,-30} {p.Price,10:F2} грн");
}

if (result.Errors.Count > 0)
{
    Console.WriteLine($"Пропущено рядків/записів: {result.Errors.Count}");
    foreach (string e in result.Errors)
    {
        Console.WriteLine($"  ! {e}");
    }
}

// Статистика імпорту одним рядком (усього / прийнято / пропущено / % помилок)
int total = result.Items.Count + result.Errors.Count;
double errorPercent = total > 0 ? (double)result.Errors.Count / total * 100 : 0;
Console.WriteLine(new string('-', 60));
Console.WriteLine($"Статистика: Усього {total} | Прийнято {result.Items.Count} | Пропущено {result.Errors.Count} | Помилок: {errorPercent:F1}%");

return 0;