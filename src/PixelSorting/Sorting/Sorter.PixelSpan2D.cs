using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using Utils;

namespace Sorting;
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
public unsafe partial class Sorter<TPixel>
    where TPixel : struct
{
    /// <summary>
    /// Custom <see cref="Span{T}"/> implementation, specialized for iterating a 2D image.
    /// </summary>
    public readonly ref struct PixelSpan2D
    {
        /// <summary>A byref or a native ptr.</summary>
        private readonly ref TPixel _reference;

        /// <summary>The side length 1.</summary>
        private readonly int _sizeU;

        /// <summary>The side length 2.</summary>
        private readonly int _sizeV;

        /// <summary>The number of elements this span operates on.</summary>
        private readonly uint _itemCount;

        /// <summary>
        /// One of these is guaranteed to be either 1 or -1.
        /// </summary>
        private readonly double _stepU, _stepV;

#if DEBUG
        private readonly Fraction _fStepU, _fStepV;
#endif

        private readonly int _offU, _offV;

        private readonly ref nint _indexerRef;


        /// <summary>
        /// The total number of items that the span operates on.
        /// </summary>
        public uint ItemCount => _itemCount;

        /// <summary>
        /// The size of the side indexed by u.
        /// </summary>
        public int SizeU => _sizeU;

        /// <summary>
        /// The size of the side indexed by v.
        /// </summary>
        public int SizeV => _sizeV;

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


        public PixelSpan2D(ref TPixel reference, nint[] indices, int maxU, int maxV, double stepU, double stepV,
            int offU, int offV)
        {
            Debug.Assert(maxU != 0 && maxV != 0);

            _sizeU = maxU;
            _sizeV = maxV;
            _offU = offU;
            _offV = offV;

            // The step dimensions need to be normalized in order for the index map to draw out a straight
            // line without any gaps.
            Debug.Assert(stepU != 0 || stepV != 0);
            var max = Math.Max(Math.Abs(stepU), Math.Abs(stepV));
            _stepU = stepU / max;
            _stepV = stepV / max;

            _itemCount = EstimateItemCount();

            // The buffer to store the index map is owned by the caller, allocating a new buffer
            // for each span creates too much overhead.
            Debug.Assert(indices.Length >= _itemCount);

            _reference = ref reference;
            _indexerRef = ref indices[0];
            
            // Calculate exact index mappings.
            // TODO: Can be optimized by storing the item count from last sort iteration's span and updating it each time a new span is created.
            // TODO: Benchmark whether it is faster precompute the indices or calculate them during runtime.
            // double u = _offU, v = _offV;
            // nint i = 0;
            // while (i < _itemCount)
            // {
            //     // Inlining the span access on the `indices` array. Skip the bounds check.
            //     // We shouldn't get a AccessViolation here, as we checked earlier, that the index map is big enough.
            //     Debug.Assert(i < indices.Length);
            //     ref var index = ref Unsafe.Add(ref _indexerRef, i++);
            //     index = (int)u + (int)v * SizeU;
            //
            //     u += _stepU;
            //     v += _stepV;
            // }
        }
        
        [Obsolete("This exists for benchmarking purposes.")]
        public PixelSpan2D(ref TPixel reference, nint[] indices, int maxU, int maxV, double stepU, double stepV,
            int offU, int offV, bool LEGACY)
        {
            Debug.Assert(maxU != 0 && maxV != 0);

            _sizeU = maxU;
            _sizeV = maxV;
            _offU = offU;
            _offV = offV;

            // The step dimensions need to be normalized in order for the index map to draw out a straight
            // line without any gaps.
            Debug.Assert(stepU != 0 || stepV != 0);
            var max = Math.Max(Math.Abs(stepU), Math.Abs(stepV));
            _stepU = stepU / max;
            _stepV = stepV / max;

            // The buffer to store the index map is owned by the caller, allocating a new buffer
            // for each span creates too much overhead.
            Debug.Assert(indices.Length >= _itemCount);

            _reference = ref reference;
            _indexerRef = ref indices[0];
            
            // Calculate exact index mappings.
            // TODO: Can be optimized by storing the item count from last sort iteration's span and updating it each time a new span is created.
            double u = offU, v = offV;
            nint i = 0;
            while (u < maxU && v < maxV && u >= 0 && v >= 0)
            {
                // Inlining the span access on the `indices` array. Skip the bounds check.
                // We shouldn't get a AccessViolation here, as we checked earlier, that the index map is big enough.
                Debug.Assert(i < indices.Length);
                ref var index = ref Unsafe.Add(ref _indexerRef, i++);
                index = (int)u + (int)v * SizeU;
            
                u += _stepU;
                v += _stepV;
            }

            _itemCount = (uint)i;
        }

        public PixelSpan2D(TPixel[] reference, nint[] indices, int maxU, int maxV, double stepU, double stepV, int offU, int offV)
            : this(ref reference[0], indices, maxU, maxV, stepU, stepV, offU, offV)
        {
        }


#if DEBUG
        public PixelSpan2D(
            TPixel[] reference, nint[] indices, 
            int maxU, int maxV, 
            Fraction fStepU, Fraction fStepV, 
            int offU, int offV)
            : this(
                ref reference[0], indices,
                maxU, maxV, 
                (double)fStepU, 
                (double)fStepV, 
                offU, offV)
        {
            // The step dimensions need to be normalized in order for the index map to draw out a 
            // continuous line without any gaps.
            Debug.Assert(!fStepU.IsZero || !fStepV.IsZero);
            var max = Fraction.Max(fStepU.Abs(), fStepV.Abs());
            _fStepU = fStepU / max;
            _fStepV = fStepV / max;
        }
#endif

        public PixelSpan2D(Span<TPixel> reference, nint[] indices, int maxU, int maxV, double stepU, double stepV, int offU, int offV)
            : this(ref reference[0], indices, maxU, maxV, stepU, stepV, offU, offV)
        {
        }

        public PixelSpan2D(void* pointer, nint[] indices, int maxU, int maxV, double stepU, double stepV, int offU, int offV)
            : this(ref *((TPixel*)pointer), indices, maxU, maxV, stepU, stepV, offU, offV)
        {
        }

        public PixelSpan2D(nint pointer, nint[] indices, int maxU, int maxV, double stepU, double stepV, int offU, int offV)
            : this(ref *((TPixel*)pointer), indices, maxU, maxV, stepU, stepV, offU, offV)
        {
        }


        /// <summary>
        /// Generates an estimate of how many items this span will operate on.
        /// Prone to floating point inaccuracy.
        /// The output value is floored, which makes it unlikely that the span operates on
        /// fewer items than the estimate.
        /// </summary>
        public uint EstimateItemCount()
        {
            Debug.Assert(_stepU != 0 || _stepV != 0);

            // Possible slots in direction u
            var uSlots = _stepU switch
            {
                > 0 => (_sizeU - _offU) / _stepU,
                < 0 => _offU / -_stepU + 1,
                _ => double.PositiveInfinity
            };

            // Possible slots in direction v
            var vSlots = _stepV switch
            {
                > 0 => (_sizeV - _offV) / _stepV,
                < 0 => _offV / -_stepV + 1,
                _ => double.PositiveInfinity
            };

            // Choose the minimum bound.
            return (uint)Math.Min(uSlots, vSlots);
        }

#if DEBUG
        /// <summary>
        /// Generates the exact number of items this span will operate on.
        /// All floating point inaccuracy is eliminated.
        /// </summary>
        public int CalculateItemCount()
        {
            Fraction u = (Fraction)_offU, v = (Fraction)_offV, sizeU = (Fraction)_sizeU, sizeV = (Fraction)_sizeV;
            int result = 0;

            while (
                u < sizeU && u >= Fraction.Zero &&
                v < sizeV && v >= Fraction.Zero)
            {
                result++;
                u += _fStepU;
                v += _fStepV;
            }

            return result;
        }
#endif

        /// <summary>
        /// Get a reference to an item by index, calculated using u, v steps from initiation.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public ref TPixel this[uint i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (i >= _itemCount)
                    throw new IndexOutOfRangeException();

                return ref
                    Unsafe.Add(ref _reference,
                        // Map the input index to the 2D array
                        Unsafe.Add(ref _indexerRef, (nint)i)
                    );
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public nint MapIndex(uint i)
        {
            double 
                u = _offU + i * _stepU,
                v = _offV + i * _stepV;

            return (int)u + (int)v * _sizeU;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public nint LookupIndex(uint i)
        {
            return Unsafe.Add(ref _indexerRef, (nint)i);
        }
    }

#if DEBUG
    public readonly struct Fraction : IAdditionOperators<Fraction, Fraction, Fraction>,
            ISubtractionOperators<Fraction, Fraction, Fraction>,
            IMultiplyOperators<Fraction, Fraction, Fraction>,
            IDivisionOperators<Fraction, Fraction, Fraction>
    {
        private readonly BigInteger _numerator;
        private readonly BigInteger _denominator;

        public Fraction(BigInteger numerator, BigInteger denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        public BigInteger Numerator
        {
            get => _numerator; 
            init => _numerator = value;
        }

        public BigInteger Denominator
        {
            get => _denominator;
            init
            {
                if (value == 0) 
                    throw new DivideByZeroException();

                // Delegate sign to the numerator.
                if (value < 0)
                {
                    value = -value;
                    _numerator = -_numerator;
                }

                _denominator = value;
            }
        }

        public bool IsZero => Numerator == 0;

        public bool IsNegative => Numerator < 0;

        public bool IsPositive => Numerator > 0;

        public static Fraction Zero => new() { Numerator = 0, Denominator = 1 };


        public static Fraction operator +(Fraction left, Fraction right) =>
            new()
            {
                Numerator = left.Numerator * right.Denominator + right.Numerator * left.Denominator,
                Denominator = left.Denominator * right.Denominator
            };

        public static Fraction operator -(Fraction left, Fraction right) =>
            new()
            {
                Numerator = left.Numerator * right.Denominator - right.Numerator * left.Denominator,
                Denominator = left.Denominator * right.Denominator
            };

        public static Fraction operator *(Fraction left, Fraction right) =>
            new()
            {
                Numerator = left.Numerator * right.Numerator,
                Denominator = left.Denominator * right.Denominator,
            };

        public static Fraction operator /(Fraction left, Fraction right) =>
            new()
            {
                Numerator = left.Numerator * right.Denominator,
                Denominator = left.Denominator * right.Numerator
            };

        public static explicit operator double(Fraction fraction) => 
            (double)fraction.Numerator / (double)fraction.Denominator;

        public Fraction Abs() => this with { Numerator = BigInteger.Abs(Numerator), };

        public static Fraction Max(Fraction left, Fraction right) => left > right ? left : right;

        public static Fraction Min(Fraction left, Fraction right) => left < right ? left : right;

        public static bool operator <(Fraction left, Fraction right)
        {
            if (left.IsZero)
                return right.IsPositive;

            if (right.IsZero)
                return left.IsNegative;

            if (left.IsNegative && right.IsPositive)
                return true;

            if (left.IsPositive && right.IsNegative)
                return false;

            if (left.IsPositive && right.IsPositive)
                // This only works if a, b, c and d are positive.
                return left.Numerator * right.Denominator < right.Numerator * left.Denominator;

            // Both are negative.
            return -left > -right && left != right;
        }

        public static bool operator <=(Fraction left, Fraction right) => left < right || left == right;

        public static bool operator >=(Fraction left, Fraction right) => left > right || left == right;

        public static bool operator ==(Fraction left, Fraction right) =>
            left.Numerator * right.Denominator == right.Numerator * left.Denominator; 

        public static bool operator !=(Fraction left, Fraction right) => !(left == right);

        public static Fraction operator -(Fraction fraction) => fraction with { Numerator = -fraction.Numerator };

        public static bool operator >(Fraction left, Fraction right) {
            if (left == right)
                return false;

            if (left.IsZero)
                return right.IsNegative;

            if (right.IsZero)
                return left.IsPositive;

            if (left.IsNegative && right.IsPositive)
                return false;

            if (left.IsPositive && right.IsNegative)
                return true;

            if (left.IsPositive && right.IsPositive)
                // This only works if a, b, c and d are positive.
                return left.Numerator * right.Denominator > right.Numerator * left.Denominator;

            // Both are negative.
            return -left < -right && left != right;
        }

        public static explicit operator Fraction(int value) => new() { Numerator = value, Denominator = 1 };
    }
#endif


}
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
public unsafe partial class Sorter<TPixel>
    where TPixel : struct
{
    /// <summary>
    /// Custom `Span<typeparamref name="TPixel"/>` implementation.
    /// </summary>
    /// <typeparam name="TPixel"></typeparam>
    public readonly ref struct UnsafePixelSpan2D
    {
        /// <summary>A byref or a native ptr.</summary>
        private readonly ref TPixel _reference;

        /// <summary>
        /// The side length 1.
        /// </summary>
        public readonly int SizeU;

        /// <summary>
        /// The side length 2.
        /// </summary>
        public readonly int SizeV;

        /// <summary>
        /// The total number of items that the span operates on.
        /// </summary>
        public readonly uint ItemCount;

        /// <summary>
        /// The step that each index takes in direction u.
        /// This can be negative.
        /// </summary>
        public double StepU { get; init; }

        /// <summary>
        /// The step that each index takes in direction v.
        /// This can be negative.
        /// </summary>
        public double StepV { get; init; }

        /// <summary>
        /// Offset in u direction.
        /// </summary>
        public int OffU { get; init; }

        /// <summary>
        /// Offset in v direction.
        /// </summary>
        public int OffV { get; init; }

        //private readonly Span<nint> _indexer;
        private readonly ref nint _indexerReference;
        //
        // /// <summary>
        // /// 
        // /// </summary>
        // /// <param name="reference"></param>
        // /// <param name="maxU">The first side length.</param>
        // /// <param name="maxV">The second side length.</param>
        // public UnsafePixelSpan2D(TPixel[] reference, nint[] indeces, int maxU, int maxV, double stepU, double stepV,
        //     int offU, int offV)
        // {
        //     Debug.Assert(indeces.Length >= FastEstimateItemCount() + 1);
        //
        //     _reference = ref reference[0];
        //     SizeU = maxU;
        //     SizeV = maxV;
        //     StepU = stepU;
        //     StepV = stepV;
        //     OffU = offU;
        //     OffV = offV;
        //     _indexerReference = ref indeces[0];
        //     ItemCount = (uint)FillIndexMap();
        // }
        //
        // /// <summary>
        // /// 
        // /// </summary>
        // /// <param name="reference"></param>
        // /// <param name="maxU">The first side length.</param>
        // /// <param name="maxV">The second side length.</param>
        // public UnsafePixelSpan2D(Span<TPixel> reference, nint[] indeces, int maxU, int maxV, double stepU, double stepV,
        //     int offU, int offV)
        // {
        //     Debug.Assert(indeces.Length >= FastEstimateItemCount() + 1);
        //
        //     _reference = ref reference[0];
        //     SizeU = maxU;
        //     SizeV = maxV;
        //     StepU = stepU;
        //     StepV = stepV;
        //     OffU = offU;
        //     OffV = offV;
        //     _indexerReference = ref indeces[0];
        //     ItemCount = FillIndexMap();
        // }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="maxU">The first side length.</param>
        /// <param name="maxV">The second side length.</param>
        public UnsafePixelSpan2D(void* pointer, nint[] indeces, int maxU, int maxV, double stepU, double stepV,
            int offU, int offV)
        {
            Debug.Assert(indeces.Length >= FastEstimateItemCount() + 1);

            _reference = ref *((TPixel*)pointer);
            SizeU = maxU;
            SizeV = maxV;
            StepU = stepU;
            StepV = stepV;
            OffU = offU;
            OffV = offV;
            _indexerReference = ref indeces[0];
            ItemCount = (uint)FillIndexMap();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="maxU">The first side length.</param>
        /// <param name="maxV">The second side length.</param>
        public UnsafePixelSpan2D(nint pointer, nint[] indeces, int maxU, int maxV, double stepU, double stepV, int offU,
            int offV)
            : this((TPixel*)pointer, indeces, maxU, maxV, stepU, stepV, offU, offV)
        {
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
            if (StepU > 0)
            {
                uSlots = Math.Round((SizeU - OffU) / StepU);
            }
            else if (StepU < 0)
            {
                uSlots = OffU / -StepU + 1;
            }
            else
            {
                uSlots = double.PositiveInfinity;
            }

            // Possible slots in direction v
            if (StepV > 0)
            {
                vSlots = Math.Round((SizeV - OffV) / StepV);
            }
            else if (StepV < 0)
            {
                vSlots = OffV / -StepV + 1;
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
        private int FillIndexMap()
        {
            // Using SIMD offers no performance boost here.
            /*
            int i = 0;
            double[] uvArr = [OffU, OffV];
            double[] stepArr = [StepU, StepV];
            double[] scalarArr = [1, SizeU];
            double[] sizerArr = [SizeU, SizeV];
            fixed (double* uvPtr = uvArr)
            fixed (double* stepPtr = stepArr)
            fixed (double* scalarPtr = scalarArr)
            fixed (double* sizerPtr = sizerArr)
            {
                Vector128<double> uv = Avx.LoadVector128(uvPtr);
                Vector128<double> step = Avx.LoadVector128(stepPtr);
                Vector128<double> scalar = Avx.LoadVector128(scalarPtr);
                Vector128<double> size = Avx.LoadVector128(sizerPtr);

                // Returns true if both elements of v1 < both elements of f2
                //static bool FastLessThan(Vector128<double> v1, Vector128<double> v2)
                //{
                //    // https://en.wikipedia.org/wiki/Double-precision_floating-point_format
                //    Vector128<double> signBit = (Vector128<ulong>.One << 63).AsDouble();
                //    var diff = Avx.Subtract(v1, v2);
                //    var sign = Avx.And(diff, signBit);
                //    return Avx.Compare(sign, signBit, )
                //}

                //double u = OffU, v = OffV;
                //while (u < SizeU && v < SizeV && u >= 0 && v >= 0)
                while (uv[0] < SizeU && uv[1] < SizeV && uv[0] >= 0 && uv[1] >= 0)
                //while (Sse. Avx.Compare(uv, size, FloatComparisonMode.OrderedLessThanNonSignaling))
                {
                    //ref nint index = ref Unsafe.Add(ref _indexerReference, (nint)(uint)i++);
                    //index = (int)u + (int)v * SizeU;
                    //u += StepU;
                    //v += StepV;

                    ref nint index = ref Unsafe.Add(ref _indexerReference, (nint)(uint)i);
                    //var floor = Avx.Floor(uv);
                    //index = (int)(uv.GetElement(0) + uv.GetElement(1) * SizeU);
                    var scaled = Avx.Multiply(Avx.Floor(uv), scalar);
                    index = (nint)(scaled.GetElement(0) + scaled.GetElement(1));
                    uv = Avx.Add(uv, step);
                    ++i;
                }
            }
            return i;
            */

            var i = 0;
            double u = OffU, v = OffV;
            while (u < SizeU && v < SizeV && u >= 0 && v >= 0)
            {
                // Inlining the span access on the `indeces` array.
                ref var index = ref Unsafe.Add(ref _indexerReference, (nint)(uint)i++);
                index = (int)u + (int)v * SizeU;
                u += StepU;
                v += StepV;
            }

            return i;
        }


        /// <summary>
        /// Get a reference to an item by index, calculated using u, v steps from initiation. 
        /// If an invalid index is requested, the whole application crashes, as such error cannot be caught.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        /// <exception cref="AccessViolationException">For invalid indeces.</exception>
        public ref TPixel this[uint i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref
                    Unsafe.Add(ref _reference,
                        // Map the input index to the 2D array
                        Unsafe.Add(ref _indexerReference, (nint)i)
                    );
            }
        }
    }
}
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type