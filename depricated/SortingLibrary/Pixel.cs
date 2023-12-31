using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingLibrary
{
    public record struct Pixel(byte R, byte G, byte B);


    public class PixelBenchmark
    {
        private SimpleComparablePixel[] _simplePixel = { new SimpleComparablePixel(0, 0, 0, 0) , new SimpleComparablePixel(255, 255, 255, 255) };
        private ComparablePixel[] _pixel = { new ComparablePixel(0, 0, 0, 0), new ComparablePixel(255, 255, 255, 255) };

        [Benchmark]
        public void SimplePixelCompare()
        {
            _simplePixel[0].CompareTo(_simplePixel[1]);
        }

        [Benchmark]
        public void PixelCompare()
        {
            _pixel[0].CompareTo(_pixel[1]);
        }
    }


    public struct SimpleComparablePixel : IComparable
    {
        public byte[] Data;
        
        
        public SimpleComparablePixel(params byte[] data)
        {
            Data = data;
        }


        public int CompareTo(object? obj)
        {
            return Data[0].CompareTo(((SimpleComparablePixel)obj!).Data[0]);
        }
    }

    public struct ComparablePixel : IComparable
    {
        public SortDirection SortDirection = SortDirection.Horizontal;
        public SortOrder SortOrder = SortOrder.Ascending;
        public SortType SortType = SortType.Red;

        public byte[] Data;
        public byte R => Data[0];


        public ComparablePixel(params byte[] data)
        {
            Data = data;
        }


        public int CompareTo(object? obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            int ret = (int)SortDirection;
            ret *= SortType switch
            {
                SortType.Red => R.CompareTo(((ComparablePixel)obj).R),

                _ => throw new NotSupportedException(),
            };
            return ret;
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

    public enum SortType
    {
        Red, Green, Blue,
        Hue, Saturation, Luminocity
    }
}
