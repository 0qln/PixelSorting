using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;

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
            public readonly int ItemCount;

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

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reference"></param>
            /// <param name="maxU">The first side length.</param>
            /// <param name="maxV">The second side length.</param>
            public UnsafePixelSpan2D(TPixel[] reference, nint[] indeces, int maxU, int maxV, double stepU, double stepV, int offU, int offV)
            {
                Debug.Assert(indeces.Length >= FastEstimateItemCount() + 1);

                _reference = ref reference[0];
                SizeU = maxU;
                SizeV = maxV;
                StepU = stepU;
                StepV = stepV;
                OffU = offU;
                _indexerReference = ref indeces[0];
                ItemCount = FillIndexMap();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reference"></param>
            /// <param name="maxU">The first side length.</param>
            /// <param name="maxV">The second side length.</param>
            public UnsafePixelSpan2D(Span<TPixel> reference, nint[] indeces, int maxU, int maxV, double stepU, double stepV, int offU, int offV)
            {
                Debug.Assert(indeces.Length >= FastEstimateItemCount() + 1);

                _reference = ref reference[0];
                SizeU = maxU;
                SizeV = maxV;
                StepU = stepU;
                StepV = stepV;
                OffU = offU;
                OffV = offV;
                _indexerReference = ref indeces[0];
                ItemCount = FillIndexMap();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reference"></param>
            /// <param name="maxU">The first side length.</param>
            /// <param name="maxV">The second side length.</param>
            public UnsafePixelSpan2D(void* pointer, nint[] indeces, int maxU, int maxV, double stepU, double stepV, int offU, int offV)
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
                ItemCount = FillIndexMap();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reference"></param>
            /// <param name="maxU">The first side length.</param>
            /// <param name="maxV">The second side length.</param>
            public UnsafePixelSpan2D(nint pointer, nint[] indeces, int maxU, int maxV, double stepU, double stepV, int offU, int offV)
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
                ItemCount = FillIndexMap();
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

                int i = 0;
                double u = OffU, v = OffV;
                while (u < SizeU && v < SizeV && u >= 0 && v >= 0)
                {
                    // Inlining the span access on the `indeces` array.
                    ref nint index = ref Unsafe.Add(ref _indexerReference, (nint)(uint)i++);
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
            /// <param name="u"></param>
            /// <param name="v"></param>
            /// <returns></returns>
            /// <exception cref="AccessViolationException">For invalid indeces.</exception>
            public ref TPixel this[int i]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return ref 
                        Unsafe.Add(ref _reference, 
                            // Map the input index to the 2D array
                            Unsafe.Add(ref _indexerReference, (nint)(uint)i)
                        );
                }
            }
        }

    }
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
}
