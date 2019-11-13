namespace savaged.mvvm.Core.Interfaces
{
    public interface ISubmittable
    {
        bool IsSubmitted { get; set; }

        void ToggleSubmitted();

        int ParentId { get; set; }

        string ParentName { get; set; }
    }
}
