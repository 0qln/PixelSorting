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

        /// <summary>
        /// The shift in either u or v direction.<br/>
        /// The direction is determined by the step size that is normalized;
        /// </summary>
        private readonly int _shift;

        /// <summary>
        /// At least one of stepU and steV is normalized to either 1 or -1.
        /// </summary>
        private readonly double _stepU, _stepV;

        /// <summary>
        /// The total number of elements in the run.
        /// </summary>
        public int ItemCount => Hi - Lo;

        /// <summary>
        /// Exclusive
        /// </summary>
        private int Hi { get; }

        /// <summary>
        /// Inclusive
        /// </summary>
        private int Lo { get; }


        public PixelSpan2DRun(ref TPixel reference, int sizeU, int sizeV, double stepU, double stepV, int shift)
        {
            _reference = ref reference;
            _sizeU = sizeU;
            _sizeV = sizeV;
            _shift = shift;
            
            // The step dimensions need to be normalized in order for the index map to draw out a straight
            // line without any gaps.
            Debug.Assert(stepU != 0 || stepV != 0);
            var max = Math.Max(Math.Abs(stepU), Math.Abs(stepV));
            _stepU = stepU / max;
            _stepV = stepV / max;

            var hasAtleastOne = false;
            for (int i = 0; i < sizeU * sizeV; i++)
            {
                if (MapIndex(i) is not null)
                {
                    hasAtleastOne = true;
                    break;
                }
            }

            if (!hasAtleastOne)
                throw new ArgumentException("Empty run.");

            int lo = 0;
            while (MapIndex(lo) is null)
            {
                lo++;
            }
            
            int hi = lo + 1;
            while (MapIndex(hi) is not null)
            {
                hi++;
            }
            
            Lo = lo;
            Hi = hi;
            
        }

        public ref TPixel this[int i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                i += Lo;

                if (i >= Hi)
                    throw new IndexOutOfRangeException();

                return ref Unsafe.Add(ref _reference, MapIndex(i)!.Value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private nint? MapIndex(int i)
        {
            double 
                u = i * _stepU,
                v = i * _stepV;

            const double tolerance = 0.00001;

            if (Math.Abs(_stepU - 1) < tolerance || Math.Abs(_stepU + 1) < tolerance)
            {
                v += _shift;
            }
            else if (Math.Abs(_stepV - 1) < tolerance || Math.Abs(_stepV + 1) < tolerance)
            {
                u += _shift;
            }
            else
            {
                throw new UnreachableException("Either stepU or stepV is not 1 or -1.");
            }
            if (u < 0 || u >= _sizeU) return null;
            if (v < 0 || v >= _sizeV) return null;

            return (nint)((int)u + (int)v * _sizeU);
        }
    }
}
