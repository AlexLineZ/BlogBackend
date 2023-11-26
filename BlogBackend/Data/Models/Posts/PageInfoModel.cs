namespace BlogBackend.Models.Posts;

public class PageInfoModel
{
    public Int32 Size;
    public Int32 Count;
    public Int32 Current;
    
    public PageInfoModel() {}

    public PageInfoModel(Int32 size, Int32 count, Int32 current)
    {
        Size = size;
        Count = count;
        Current = current;
    }
}