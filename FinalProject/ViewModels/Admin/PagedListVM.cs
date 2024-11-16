namespace FinalProject.ViewModels.Admin
{
    public class PagedListVM<T>
    {
        public int PageNumber { get; set; } 
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public IEnumerable<T> Items { get; set; } 

        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}
