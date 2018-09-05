namespace SixNations.Desktop.Interfaces
{
    public interface IRequirement
    {
        int Estimation { get; set; }
        int Id { get; }
        int Priority { get; set; }
        string Release { get; set; }
        int Status { get; set; }
        string Story { get; set; }
    }
}