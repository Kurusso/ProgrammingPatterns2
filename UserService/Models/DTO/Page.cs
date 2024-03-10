namespace UserService.Models.DTO;

public struct PageInfo
{
    public PageInfo(int page, int total, int pageSize)
    {
        CurrentPage = page;
        PagesTotal = total/pageSize;
        if (total % pageSize != 0)
            PagesTotal++;

    }

    public int CurrentPage { get; set; } = 0;
    public int PagesTotal { get; set; } = 0;
}


public class Page<T>
{
    public PageInfo PageInfo { get; set; }
    public ICollection<T> Items { get; set; } = [];
}