﻿using System.Runtime.CompilerServices;

namespace Sorting
{
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    public unsafe partial class Sorter<TPixel>
        where TPixel : struct
    {

        /// <summary>
        /// Custom `Span<typeparamref name="TPixel"/>` implementation.
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        public readonly ref struct PixelSpan2D
        {
            /// <summary>A byref or a native ptr.</summary>
            internal readonly ref TPixel _reference;
            /// <summary>The side length 1.</summary>
            private readonly int _sizeU;
            /// <summary>The side length 2.</summary>
            private readonly int _sizeV;
            /// <summary>The number of elements this span operates on.</summary>
            private readonly int _itemCount;

            private readonly double _stepU, _stepV;
            private readonly int _offU, _offV;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reference"></param>
            /// <param name="maxU">The first side length.</param>
            /// <param name="maxV">The second side length.</param>
            public PixelSpan2D(TPixel[] reference, int maxU, int maxV, double stepU, double stepV, int offU, int offV)
            {
                _reference = ref reference[0];
                _sizeU = maxU;
                _sizeV = maxV;
                _stepU = stepU;
                _stepV = stepV;
                _offU = offU;
                _offV = offV;
                _itemCount = FastEstimateItemCount();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reference"></param>
            /// <param name="maxU">The first side length.</param>
            /// <param name="maxV">The second side length.</param>
            public PixelSpan2D(Span<TPixel> reference, int maxU, int maxV, double stepU, double stepV, int offU, int offV)
            {
                _reference = ref reference[0];
                _sizeU = maxU;
                _sizeV = maxV;
                _stepU = stepU;
                _stepV = stepV;
                _offU = offU;
                _offV = offV;
                _itemCount = FastEstimateItemCount();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reference"></param>
            /// <param name="maxU">The first side length.</param>
            /// <param name="maxV">The second side length.</param>
            public PixelSpan2D(void* pointer, int maxU, int maxV, double stepU, double stepV, int offU, int offV)
            {
                _reference = ref *((TPixel*)pointer);
                _sizeU = maxU;
                _sizeV = maxV;
                _stepU = stepU;
                _stepV = stepV;
                _offU = offU;
                _offV = offV;
                _itemCount = FastEstimateItemCount();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reference"></param>
            /// <param name="maxU">The first side length.</param>
            /// <param name="maxV">The second side length.</param>
            public PixelSpan2D(nint pointer, int maxU, int maxV, double stepU, double stepV, int offU, int offV)
            {
                _reference = ref *((TPixel*)pointer);
                _sizeU = maxU;
                _sizeV = maxV;
                _stepU = stepU;
                _stepV = stepV;
                _offU = offU;
                _offV = offV;
                _itemCount = FastEstimateItemCount();
            }

            /// <summary>
            /// Prone to floating point inaccuracy;
            /// this sometimes estimates one item to few.
            /// </summary>
            /// <returns></returns>
            public int FastEstimateItemCount()
            {
                double uSlots, vSlots;

                // Possible slots in direction u
                if (_stepU > 0)
                {
                    uSlots = Math.Round((_sizeU - _offU) / _stepU);
                }
                else if (_stepU < 0)
                {
                    uSlots = _offU / -_stepU + 1;
                }
                else
                {
                    uSlots = double.PositiveInfinity;
                }

                // Possible slots in direction v
                if (_stepV > 0)
                {
                    vSlots = Math.Round((_sizeV - _offV) / _stepV);
                }
                else if (_stepV < 0)
                {
                    vSlots = _offV / -_stepV + 1;
                }
                else
                {
                    vSlots = double.PositiveInfinity;
                }

                // Choose the minimum bound.
                return (int)(Math.Min(uSlots, vSlots));
            }

            /// <summary>
            /// Generates an estimate of how many items this span will operate on.
            /// </summary>
            public int EstimateItemCount()
            {
                int result = 0;
                double u = _offU, v = _offV;
                while (u < _sizeU && v < _sizeV && u >= 0 && v >= 0) {
                    result++;
                    u += _stepU;
                    v += _stepV;
                }
                return result;
            }

            /// <summary>
            /// Get a reference to an item by index, calculated using u, v steps from initiation.
            /// </summary>
            /// <param name="u"></param>
            /// <param name="v"></param>
            /// <returns></returns>
            /// <exception cref="IndexOutOfRangeException"></exception>
            public ref TPixel this[int i]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    if (i >= _itemCount || i < 0)
                        throw new IndexOutOfRangeException();

                    int u = (int)(i * _stepU) + _offU;
                    int v = (int)(i * _stepV) + _offV;

                    int index = u + v * _sizeU;

                    return ref Unsafe.Add(ref _reference, (nint)(uint)(index));
                }
            }

            /// <summary>
            /// The total number of items that the span operates on.
            /// </summary>
            public int ItemCount
            {
                get
                {
                    return _itemCount;
                }
            }

            /// <summary>
            /// The size of the side indexed by u.
            /// </summary>
            public int SizeU
            {
                get
                {
                    return _sizeU;
                }
            }

            /// <summary>
            /// The size of the side indexed by v.
            /// </summary>
            public int SizeV
            {
                get
                {
                    return _sizeV;
                }
            }

            /// <summary>
            /// The step that each index takes in direction u.
            /// This can be negative.
            /// </summary>
            public double StepU => _stepU;

            /// <summary>
            /// The step that each index takes in direction v.
            /// This can be negative.
            /// </summary>
            public double StepV => _stepV;
        }

    }
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
}
