


namespace SortingLibrary;

public class Sorter
{
    public SortType SortType { get; set; } = SortType.Red;
    public SortDirection SortDirection { get; set; } = SortDirection.Horizontal;
    public SortOrder SortOrder { get; set; } = SortOrder.Ascending;

    private readonly int _imageWidth, _imageHeight, _imageStride, _bytesPerPixel;
    private byte[] _imageData;
    private ComparablePixel[] _pixels;
    private SimpleComparablePixel[] _simplePixels;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="pixelData">Expected to be 3 bytes per pixel: Red, Green, Blue</param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public Sorter(byte[] pixelData, int width, int height, int stride)
    {
        _imageStride = stride;
        _imageHeight = height;
        _imageWidth = width;
        _imageData = pixelData;
        _bytesPerPixel = stride / width;

        _pixels = _imageData.Chunk(_bytesPerPixel).Select(x => new ComparablePixel(x)).ToArray();
        _simplePixels = _imageData.Chunk(_bytesPerPixel).Select(x => new SimpleComparablePixel(x)).ToArray();
    }


    private Span<ComparablePixel> BuildSpanFromPixels(int scanIdx)
    {
        if (SortDirection == SortDirection.Horizontal)
        {
            return new Span<ComparablePixel>(_pixels.Chunk(_imageWidth).ElementAt(scanIdx));
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private Span<ComparablePixel> BuildSpanFromBytes(int scanIdx)
    {
        byte[] scan;
        if (SortDirection == SortDirection.Horizontal)
        {
            scan = _imageData.Chunk(_imageStride).ElementAt(scanIdx);
        }
        else
        {
            throw new NotImplementedException();
        }

        return new Span<ComparablePixel>(
            scan
            .Chunk(_bytesPerPixel)
            .Select(x => new ComparablePixel(x))
            .ToArray()
        );
    }

    private Span<SimpleComparablePixel> BuildSimpleSpanFromPixels(int scanIdx)
    {
        if (SortDirection == SortDirection.Horizontal)
        {
            return new Span<SimpleComparablePixel>(_simplePixels.Chunk(_imageWidth).ElementAt(scanIdx));
        }
        else
        {
            throw new NotImplementedException();
        }
    }
    private Span<SimpleComparablePixel> BuildSimpleSpanFromBytes(int scanIdx)
    {
        byte[] scan;
        if (SortDirection == SortDirection.Horizontal)
        {
            scan = _imageData.Chunk(_imageStride).ElementAt(scanIdx);
        }
        else
        {
            throw new NotImplementedException();
        }

        return new Span<SimpleComparablePixel>(
            scan
            .Chunk(_bytesPerPixel)
            .Select(x => new SimpleComparablePixel(x))
            .ToArray()
        );
    }


    public void StdSortBytes()
    {
        if (SortOrder == SortOrder.Ascending)
        {
            for (int scan = 0; scan < (SortDirection == SortDirection.Horizontal ? _imageWidth : _imageHeight); scan++)
            {
                BuildSpanFromBytes(scan).Sort();
            }
        }
        //else
        //{
        //    for (int scan = (SortDirection == SortDirection.Horizontal ? _imageWidth : _imageHeight) - 1; scan >= 0; scan--)
        //    {
        //        BuildSpanFromBytes(scan).Sort();
        //    }
        //}
    }
    public void StdSortPixels()
    {
        if (SortOrder == SortOrder.Ascending)
        {
            for (int scan = 0; scan < (SortDirection == SortDirection.Horizontal ? _imageWidth : _imageHeight); scan++)
            {
                BuildSpanFromPixels(scan).Sort();
            }
        }
    }
    public void SimpleStdSortBytes()
    {
        if (SortOrder == SortOrder.Ascending)
        {
            for (int scan = 0; scan < (SortDirection == SortDirection.Horizontal ? _imageWidth : _imageHeight); scan++)
            {
                BuildSimpleSpanFromBytes(scan).Sort();
            }
        }
    }
    public void SimpleStdSortPixels()
    {
        if (SortOrder == SortOrder.Ascending)
        {
            for (int scan = 0; scan < (SortDirection == SortDirection.Horizontal ? _imageWidth : _imageHeight); scan++)
            {
                BuildSimpleSpanFromPixels(scan).Sort();
            }
        }
    }



    #region QuickSort
    public void QuickSort()
    {

    }
    private void QuickSort(int left, int right)
    {
        if (left < right)
        {

        }
    }
    private void Partition()
    {

    }
    #endregion

    #region RadixSort

    #endregion
}
