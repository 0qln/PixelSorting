using System.Runtime.CompilerServices;

namespace Sorting.Pixels._24;
// TODO: make this work

/// <summary>
/// Currently unusable, due to the `ref` keyword, which is neccessary, but prevents
/// the structure from being used as a type argument.
/// </summary>
public readonly unsafe ref struct Pixel24bitSpan
{
    private readonly ref byte _reference;


    public unsafe Pixel24bitSpan(ref byte reference)
    {
            _reference = ref reference;
        }


    public unsafe ref byte R
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
                return ref _reference;
            }
    }

    public unsafe ref byte G
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
                return ref Unsafe.Add(ref _reference, 1);
            }
    }

    public unsafe ref byte B
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
                return ref Unsafe.Add(ref _reference, 2);
            }
    }
}