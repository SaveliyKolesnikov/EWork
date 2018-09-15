using System.Linq;

namespace EWork.ViewModels
{
    public class AdminPageViewModel<T> where T : class
    {
        public AdminPageViewModel(IQueryable<T> items, string searchString = "")
            => (Items, SearchString) = (items, searchString);

        public IQueryable<T> Items { get; }
        public string SearchString { get; }
    }
}
