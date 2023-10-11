namespace SortingLibrary;


public record struct Pixel_24bit(byte B, byte G, byte R);


// (so = SortOrder)
// (st = SortType)
public class Comparer24bit_soA_stR : IComparer<Pixel_24bit>
{
    public int Compare(Pixel_24bit a, Pixel_24bit b)
    {
        return a.R.CompareTo(b.R);
    }
}
public class Comparer24bit_soA_stB : IComparer<Pixel_24bit>
{
    public int Compare(Pixel_24bit a, Pixel_24bit b)
    {
        return a.B.CompareTo(b.B);
    }
}

public enum SortDirection
{
    Horizontal, Vertical
}

public enum SortOrder
{
    Ascending, Descending
}

public enum SortType
{
    Red, Green, Blue,
    Hue, Saturation, Luminocity
}

//public enum SortDirection
//{
//    North = 8, 
//    East = -1,
//    South = -8, 
//    West = 1,

//    NorthEast = North + East,
//    SouthEast = South + East,
//    NorthWest = North + West,
//    SouthWest = South + West,
//}
