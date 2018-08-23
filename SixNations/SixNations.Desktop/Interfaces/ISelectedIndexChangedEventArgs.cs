namespace SixNations.Desktop.Interfaces
{
    public interface ISelectedIndexChangedEventArgs
    {
        int NewValue { get; }
        int OldValue { get; }
    }
}