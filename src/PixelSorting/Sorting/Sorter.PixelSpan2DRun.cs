using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sorting;

#pragma  warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

public unsafe partial class Sorter<TPixel>
    where TPixel : struct
{
    /// <summary>
    /// A span from an image. <br/>
    /// Allows for precise indexing along some line in the image.
    /// </summary>
    public readonly ref struct PixelSpan2DRun
    {
        /// <summary>A byref or a native ptr.</summary>
        private readonly ref TPixel _reference;

        /// <summary>The side length 1.</summary>
        private readonly int _sizeU;

        /// <summary>The side length 2.</summary>
        private readonly int _sizeV;

        private readonly int _offU, _offV;

        /// <summary>
        /// At least one of stepU and steV is normalized to either 1 or -1.
        /// </summary>
        private readonly double _stepU, _stepV;

        /// <summary>
        /// The total number of elements in the run.
        /// </summary>
        public uint ItemCount => _hi - _lo;

        /// <summary>
        /// Exclusive
        /// </summary>
        private readonly uint _hi;

        /// <summary>
        /// Inclusive
        /// </summary>
        private readonly uint _lo;


        /// <summary>
        /// Creates a new span.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="sizeU"></param>
        /// <param name="sizeV"></param>
        /// <param name="stepU"></param>
        /// <param name="stepV"></param>
        /// <param name="offU"></param>
        /// <param name="offV"></param>
        /// <param name="inverseIndexing"></param>
        public PixelSpan2DRun(TPixel* reference, int sizeU, int sizeV, double stepU, double stepV, int offU, int offV, bool inverseIndexing = false)
            : this(ref Unsafe.AsRef<TPixel>(reference), sizeU, sizeV, stepU, stepV, offU, offV, inverseIndexing)
        {
        }

        /// <summary>
        /// Creates a new span.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="sizeU"></param>
        /// <param name="sizeV"></param>
        /// <param name="stepU"></param>
        /// <param name="stepV"></param>
        /// <param name="offU"></param>
        /// <param name="offV"></param>
        /// <param name="inverseIndexing"></param>
        public PixelSpan2DRun(ref TPixel reference, int sizeU, int sizeV, double stepU, double stepV, int offU, int offV, bool inverseIndexing = false)
        {
            if (inverseIndexing)
            {
                stepU *= -1;
                stepV *= -1;
                offU *= -1;
                offV *= -1;
                offU += sizeU - 1;
                offV += sizeV - 1;
            }

            _reference = ref reference;
            _sizeU = sizeU;
            _sizeV = sizeV;
            _offU = offU;
            _offV = offV;
            
            // The step dimensions need to be normalized in order for the index map to draw out a straight
            // line without any gaps.
            Debug.Assert(stepU != 0 || stepV != 0);
            var max = Math.Max(Math.Abs(stepU), Math.Abs(stepV));
            _stepU = stepU / max;
            _stepV = stepV / max;


            // TODO: Optimize these loops away
            // TODO---------------------------

            var hasAtLeastOne = false;
            for (uint i = 0; i < sizeU * sizeV; i++)
            {
                if (TryMapIndex(i))
                {
                    hasAtLeastOne = true;
                    break;
                }
            }

            if (!hasAtLeastOne)
            {
                _hi = _lo = 0;
                return;
            }

            uint lo = 0;
            while (!TryMapIndex(lo))
                lo++;

            uint hi = lo + 1;
            while (TryMapIndex(hi))
                hi++;
            
            // TODO---------------------------

            _lo = lo;
            _hi = hi;
        }

        /// <summary>
        /// Creates a slice of the original span.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="lo"></param>
        /// <param name="hi"></param>
        public PixelSpan2DRun(PixelSpan2DRun original, uint lo, uint hi)
        {
            _reference = ref original._reference;
            _stepU = original._stepU;
            _stepV = original._stepV;
            _sizeU = original._sizeU;
            _sizeV = original._sizeV;
            _offU = original._offU;
            _offV = original._offV;
            _lo = original._lo + lo;
            _hi = original._lo + hi;

            Debug.Assert(_hi <= original._hi);
            Debug.Assert(_lo >= original._lo);
        }
        
        // TODO: unit test this
        /// <summary>
        /// Finds the next run starting at <paramref name="idx"/>.
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="threshold"></param>
        /// <param name="idx"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool NextRun(IComparer<TPixel> comparer, TPixel threshold, ref uint idx, out PixelSpan2DRun result)
        {
            // Find lo.
            var lo = idx;
            while (lo < ItemCount && comparer.Compare(this[lo], threshold) < 0) ++lo;

            // Find hi.
            var hi = lo;
            while (hi < ItemCount && comparer.Compare(this[hi], threshold) >= 0) ++hi;

            idx = hi;
            result = new(this, lo, hi);
            return lo < ItemCount;
        }

        /// <summary>
        /// Gets the pixel at <paramref name="i"/>.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public ref TPixel this[uint i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                i += _lo;

                if (i >= _hi)
                    throw new IndexOutOfRangeException();

                return ref Unsafe.Add(ref _reference, MapIndex(i));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint MapIndex(uint i)
        {
            double 
                u = i * _stepU + _offU,
                v = i * _stepV + _offV;

            return (uint)((int)u + (int)v * _sizeU);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryMapIndex(uint i)
        {
            double 
                u = i * _stepU + _offU,
                v = i * _stepV + _offV;

            return !(u < 0) && !(u >= _sizeU) && 
                   !(v < 0) && !(v >= _sizeV);
        }
    }
}
