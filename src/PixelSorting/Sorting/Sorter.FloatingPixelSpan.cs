﻿using System.Runtime.CompilerServices;

namespace Sorting;
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
public unsafe partial class Sorter<TPixel>
    where TPixel : struct
{
    //
    // /// <summary>
    // /// Custom `Span<typeparamref name="TPixel"/>` implementation.
    // /// </summary>
    // /// <typeparam name="TPixel"></typeparam>
    // public readonly ref struct FloatingPixelSpan
    // {
    //     /// <summary>A byref or a native ptr.</summary>
    //     internal readonly ref TPixel _reference;
    //     /// <summary>The number of elements this Span operates on.</summary>
    //     private readonly int _items;
    //     /// <summary>The number that controls how many where the next elmenet is when indexing.</summary>
    //     private readonly double _step;
    //
    //
    //     public FloatingPixelSpan(TPixel[] reference, double step, int lo, int hi)
    //     {
    //         _reference = ref reference[lo];
    //         var size = hi - lo;
    //         _items = (int)(size / step + (size % step == 0 ? 0 : 1));
    //         _step = step;
    //     }
    //
    //     public FloatingPixelSpan(Span<TPixel> reference, double step, int lo, int hi)
    //     {
    //         _reference = ref reference[lo];
    //         var size = hi - lo;
    //         _items = (int)(size / step + (size % step == 0 ? 0 : 1));
    //         _step = step;
    //     }
    //
    //     public FloatingPixelSpan(void* pointer, double step, int lo, int hi)
    //     {
    //         _reference = ref *((TPixel*)pointer + lo);
    //         var size = hi - lo;
    //         _items = (int)(size / step + (size % step == 0 ? 0 : 1));
    //         _step = step;
    //     }
    //
    //     public ref TPixel this[int index]
    //     {
    //         [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //         get
    //         {
    //             if ((uint)index >= (uint)_items)
    //                 throw new IndexOutOfRangeException();
    //             return ref Unsafe.Add(ref _reference, (nint)(uint)(index * _step));
    //         }
    //     }
    //
    //     public int ItemCount
    //     {
    //         get => _items;
    //     }
    //
    //     public double Step
    //     {
    //         get => _step;
    //     }
    // }

}
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type