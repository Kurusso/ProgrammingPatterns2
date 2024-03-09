namespace UserService.Models.DTO;

public struct PageInfo
{
    public PageInfo(int page, int total, int pageSize)
    {
        Size = total;
        RangeStart =  pageSize * (page - 1);
        RangeEnd = Math.Min(RangeStart + pageSize, total);
    }

    public int RangeStart { get; set; } = 0;
    public int RangeEnd { get; set; } = 0;
    public int Size { get; set; } = 0;
}


public class Page<T>
{
    public PageInfo PageInfo { get; set; }
    public ICollection<T> Items { get; set; } = [];
}