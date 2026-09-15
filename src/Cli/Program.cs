using System.Runtime.InteropServices;
using System.Text.Json;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Перевіряємо, чи є прапорець "--json" в аргументах
bool isJson = args.Contains("--json");

var appInfo = new
{
    Application = "CrossApp",
    Student = "Пинчук Марія, ФЕІ-33",
    OSDescription = RuntimeInformation.OSDescription,
    OSVersion = Environment.OSVersion.ToString(),
    ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
    DotNetVersion = Environment.Version.ToString(),
    Framework = RuntimeInformation.FrameworkDescription,
    AppDirectory = AppContext.BaseDirectory,
    CurrentDirectory = Environment.CurrentDirectory,
    Domain = "Замовлення (клієнт, товар, замовлення, рядок замовлення)"
};

if (isJson)
{
    // Виводимо все одним JSON-рядком
    string jsonString = JsonSerializer.Serialize(appInfo);
    Console.WriteLine(jsonString);
}
else
{
    // Звичайний вивід
    Console.WriteLine("CrossApp — практикум з крос-платформного програмування");
    Console.WriteLine($"Студентка: {appInfo.Student}");
    Console.WriteLine(new string('-', 52));
    Console.WriteLine($"ОС (OSDescription): {appInfo.OSDescription}");
    Console.WriteLine($"ОС (Environment) : {appInfo.OSVersion}");
    Console.WriteLine($"Архітектура процесу: {appInfo.ProcessArchitecture}");
    Console.WriteLine($"Версія .NET (CLR): {appInfo.DotNetVersion}");
    Console.WriteLine($"Runtime           : {appInfo.Framework}");
    Console.WriteLine($"Каталог застосунку: {appInfo.AppDirectory}");
    Console.WriteLine($"Поточний каталог : {appInfo.CurrentDirectory}");
    Console.WriteLine(new string('-', 52));
    Console.WriteLine($"Предметна область: {appInfo.Domain}");
}