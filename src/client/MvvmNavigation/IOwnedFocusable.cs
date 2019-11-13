namespace savaged.mvvm.Navigation
{
    public interface IOwnedFocusable : IFocusable
    {
        IFocusable Owner { get; set; }
    }
}
