using System.Collections.Generic;

namespace EWork.ViewModels
{
    public class AdminPageViewModel<T> where T : class
    {
        public AdminPageViewModel(IEnumerable<T> items, int takeAmount, string searchString = "")
            => (Items, TakeAmount, SearchString) = (items, takeAmount, searchString);

        public IEnumerable<T> Items { get; }
        public string SearchString { get; }
        public int TakeAmount { get; }
    }
}
