using Core.Abstractions;
using Core.Events;

namespace Cli.Notifications;

public sealed class FileSink : INotificationSink
{
    private readonly string _logPath;

    public FileSink(string logPath)
    {
        _logPath = logPath ?? throw new ArgumentNullException(nameof(logPath));
        Directory.CreateDirectory(Path.GetDirectoryName(_logPath)!);
    }

    public void Notify(OrderChangedEventArgs e)
    {
        string line = $"[FileSink]    {e.At:HH:mm:ss} {e.Kind,-15} {e.OrderId} | позицій: {e.LinesCount,2} -> сума: {e.Total,10:F2} грн";
        File.AppendAllText(_logPath, line + Environment.NewLine);
    }
}