namespace BlogBackend.Validation;

public static class QueryValidator
{
    public static Boolean CheckValidDataPost(Int32 page, Int32 size, Int32? min, Int32? max)
    {
        if (size < 1)
        {
            throw new ArgumentOutOfRangeException("Page must be greater than or equal to 1");
        }

        if (page < 1)
        {
            throw new ArgumentOutOfRangeException("Page must be greater than or equal to 1");
        }
        
        if (min < 0)
        {
            throw new ArgumentOutOfRangeException("Min must be greater than or equal to 0");
        }
        
        if (max < 0)
        {
            throw new ArgumentOutOfRangeException("Max must be greater than or equal to 0");
        }

        return true;
    }
    
    public static Boolean CheckValidDataCommunity(Int32 page, Int32 size)
    {
        if (size < 1)
        {
            throw new ArgumentOutOfRangeException("Page must be greater than or equal to 1");
        }

        if (page < 1)
        {
            throw new ArgumentOutOfRangeException("Page must be greater than or equal to 1");
        }

        return true;
    }
}