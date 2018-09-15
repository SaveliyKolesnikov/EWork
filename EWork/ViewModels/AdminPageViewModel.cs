using System.Linq;

namespace EWork.ViewModels
{
    public class AdminPageViewModel<T> where T : class
    {
        public AdminPageViewModel(IQueryable<T> items, string searchString = "")
        {
            Items = items;
            SearchString = searchString;
        }

        public IQueryable<T> Items { get; }
        public string SearchString { get; }
    }
}
