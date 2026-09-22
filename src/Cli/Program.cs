using Core;
using System.Text.Json;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Перевіряємо, чи є прапорець "--json" в аргументах
bool isJson = args.Contains("--json");

// Збираємо звіт із бібліотеки Core
EnvironmentReport report = EnvironmentInfo.Collect();

if (isJson)
{
    // Виводимо все одним JSON-рядком
    string jsonString = JsonSerializer.Serialize(report);
    Console.WriteLine(jsonString);
}
else
{
    // Звичайний вивід
    Console.WriteLine($"{report.Application} — практикум з крос-платформного програмування");
    Console.WriteLine($"Студентка: {report.Student}");
    Console.WriteLine(new string('-', 52));
    Console.WriteLine($"ОС (OSDescription) : {report.OsDescription}");
    Console.WriteLine($"ОС (Environment)   : {report.OsVersion}");
    Console.WriteLine($"Архітектура процесу: {report.ProcessArchitecture}");
    Console.WriteLine($"Версія .NET (CLR)  : {report.DotNetVersion}");
    Console.WriteLine($"Runtime            : {report.FrameworkDescription}");
    Console.WriteLine($"RID (визначено)    : {report.DetectedRid}");
    Console.WriteLine($"RID (від .NET)     : {report.ReportedRid}");
    Console.WriteLine($"Каталог застосунку : {report.BaseDirectory}");
    Console.WriteLine(new string('-', 52));
    Console.WriteLine($"Предметна область  : {report.Domain}");
}