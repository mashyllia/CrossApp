using Core.Abstractions;
using Core.Events;

namespace Cli.Notifications;

public sealed class ConsoleSink : INotificationSink
{
    public void Notify(OrderChangedEventArgs e)
    {
        Console.WriteLine($"[ConsoleSink] {e.At:HH:mm:ss} {e.Kind,-15} {e.OrderId} | позицій: {e.LinesCount,2} -> сума: {e.Total,10:F2} грн");
    }
}