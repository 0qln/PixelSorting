using System.Runtime.CompilerServices;

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
            /// <summary>The greater side length.</summary>
            private readonly int _sizeU;
            /// <summary>The smaller side length.</summary>
            private readonly int _sizeV;
            /// <summary>The number of elements this Span operates on.</summary>
            // private readonly int _items;
            
            private readonly int _collectionSize;
            
            private readonly double _stepU, _stepV;
            private readonly int _offU, _offV;


            /// <summary>
            /// 
            /// </summary>
            /// <param name="reference"></param>
            /// <param name="maxU">The greater side length.</param>
            /// <param name="maxV">The smaller side length.</param>
            public PixelSpan2D(TPixel[] reference, int maxU, int maxV, double stepU, double stepV, int offU, int offV)
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(maxU, maxV);

                _reference = ref reference[0];
                _sizeU = maxU;
                _sizeV = maxV;
                _stepU = stepU;
                _stepV = stepV;
                _offU = offU;
                _offV = offV;
                _collectionSize = _sizeU * _sizeV;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reference"></param>
            /// <param name="maxU">The greater side length.</param>
            /// <param name="maxV">The smaller side length.</param>
            public PixelSpan2D(Span<TPixel> reference, int maxU, int maxV, double stepU, double stepV, int offU, int offV)
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(maxU, maxV);

                _reference = ref reference[0];
                _sizeU = maxU;
                _sizeV = maxV;
                _stepU = stepU;
                _stepV = stepV;
                _offU = offU;
                _offV = offV;
                _collectionSize = _sizeU * _sizeV;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reference"></param>
            /// <param name="maxU">The greater side length.</param>
            /// <param name="maxV">The smaller side length.</param>
            public PixelSpan2D(void* pointer, int maxU, int maxV, double stepU, double stepV, int offU, int offV)
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(maxU, maxV);

                _reference = ref *((TPixel*)pointer);
                _sizeU = maxU;
                _sizeV = maxV;
                _stepU = stepU;
                _stepV = stepV;
                _offU = offU;
                _offV = offV;
                _collectionSize = _sizeU * _sizeV;
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
                    int u = (int)(i * _stepU) + _offU;
                    int v = (int)(i * _stepV) + _offV;

                    if (u >= _sizeU || v >= _sizeV || u < 0 || v < 0)
                        throw new IndexOutOfRangeException();

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
                    // TODO: test this: 
                    // double deltaU = _sizeU - _offU;
                    // double deltaV = _sizeV - _offV;
                    // int stepsU = (int)Math.Ceiling(deltaU / _stepU);
                    // int stepsV = (int)Math.Ceiling(deltaV / _stepV);
                    // return Math.Min(stepsU, stepsV);
                    int result = 0;
                    double u = _offU, v = _offV;
                    while (u < _sizeU && v < _sizeV && u >= 0 && v >= 0) {
                        result++;
                        u += _stepU;
                        v += _stepV;
                    }
                    return result;
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

            public double StepU => _stepU;
            public double StepV => _stepV;
        }

    }
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
}
