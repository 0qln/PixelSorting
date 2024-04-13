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
            /// <summary>The greater side length.</summary>
            private readonly int _sizeU;
            /// <summary>The smaller side length.</summary>
            private readonly int _sizeV;
            /// <summary>The number of elements this Span operates on.</summary>
            private readonly int _items;
            
            private readonly float _stepU, _stepV;


            /// <summary>
            /// 
            /// </summary>
            /// <param name="reference"></param>
            /// <param name="maxU">The greater side length.</param>
            /// <param name="maxV">The smaller side length.</param>
            public PixelSpan2D(TPixel[] reference, int maxU, int maxV, float stepU, float stepV)
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(maxU, maxV);

                _reference = ref reference[0];
                _sizeU = maxU;
                _sizeV = maxV;
                _stepU = stepU;
                _stepV = stepV;
                _items = _sizeU * _sizeV;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reference"></param>
            /// <param name="maxU">The greater side length.</param>
            /// <param name="maxV">The smaller side length.</param>
            public PixelSpan2D(Span<TPixel> reference, int maxU, int maxV, float stepU, float stepV)
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(maxU, maxV);

                _reference = ref reference[0];
                _sizeU = maxU;
                _sizeV = maxV;
                _stepU = stepU;
                _stepV = stepV;
                _items = _sizeU * _sizeV;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reference"></param>
            /// <param name="maxU">The greater side length.</param>
            /// <param name="maxV">The smaller side length.</param>
            public PixelSpan2D(void* pointer, int maxU, int maxV, float stepU, float stepV)
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(maxU, maxV);

                _reference = ref *((TPixel*)pointer);
                _sizeU = maxU;
                _sizeV = maxV;
                _stepU = stepU;
                _stepV = stepV;
                _items = _sizeU * _sizeV;
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
                    return ref this[(int)(i * _stepU), (int)(i * _stepV)];
                }
            }

            /// <summary>
            /// Get a reference to an item by it's u and v indeces.
            /// </summary>
            /// <param name="u"></param>
            /// <param name="v"></param>
            /// <returns></returns>
            /// <exception cref="IndexOutOfRangeException"></exception>
            public ref TPixel this[int u, int v]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    int index = u * _sizeU + v;
                    if ((uint)index >= (uint)_items)
                        throw new IndexOutOfRangeException();
                    return ref Unsafe.Add(ref _reference, (nint)(uint)(index));
                }
            }

            /// <summary>
            /// The total number of items that the span operates on.
            /// </summary>
            public int ItemCount
            {
                get => _items;
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

            public float StepU => _stepU;
            public float StepV => _stepV;
        }

    }
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
}