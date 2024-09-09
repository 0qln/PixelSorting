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
        public uint ItemCount => Hi - Lo;

        /// <summary>
        /// Exclusive
        /// </summary>
        private uint Hi { get; }

        /// <summary>
        /// Inclusive
        /// </summary>
        private uint Lo { get; }


        public PixelSpan2DRun(TPixel* reference, int sizeU, int sizeV, double stepU, double stepV, int offU, int offV, bool inverseIndexing = false)
            : this(ref Unsafe.AsRef<TPixel>(reference), sizeU, sizeV, stepU, stepV, offU, offV, inverseIndexing)
        {
        }

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
                Hi = Lo = 0;
                return;
            }

            uint lo = 0;
            while (!TryMapIndex(lo))
                lo++;

            uint hi = lo + 1;
            while (TryMapIndex(hi))
                hi++;
            
            // TODO---------------------------

            Lo = lo;
            Hi = hi;
        }

        public ref TPixel this[uint i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                i += Lo;

                if (i >= Hi)
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
