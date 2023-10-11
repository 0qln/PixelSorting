namespace SortingLibrary;


public record struct Pixel_24bit(byte B, byte G, byte R);


public class Comparer24bit
{
    public class Descending
    {
        public class Red : IComparer<Pixel_24bit>
        {
            public int Compare(Pixel_24bit a, Pixel_24bit b)
            {
                return b.R.CompareTo(a.R);
            }
        }

        public class Blue : IComparer<Pixel_24bit>
        {
            public int Compare(Pixel_24bit a, Pixel_24bit b)
            {
                return b.B.CompareTo(a.B);
            }
        }
    }

    public class Ascending
    {
        public class Red : IComparer<Pixel_24bit>
        {
            public int Compare(Pixel_24bit a, Pixel_24bit b)
            {
                return a.R.CompareTo(b.R);
            }
        }

        public class Blue : IComparer<Pixel_24bit>
        {
            public int Compare(Pixel_24bit a, Pixel_24bit b)
            {
                return a.B.CompareTo(b.B);
            }
        }
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
