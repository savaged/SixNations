using System;
using System.Threading.Tasks;

namespace SixNations.Desktop.Interfaces
{
    public interface IAsyncViewModel
    {
        Task LoadAsync();
    }
}
