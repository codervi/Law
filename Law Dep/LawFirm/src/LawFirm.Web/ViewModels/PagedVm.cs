namespace LawFirm.Web.ViewModels;

public class PagedVm<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public string? Status { get; set; }
}
