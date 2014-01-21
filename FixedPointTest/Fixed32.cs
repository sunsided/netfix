using System.Diagnostics.Contracts;

namespace ConsoleApplication1
{
    /// <summary>
    /// Struct Fixed32.
    /// <para>
    /// 64bit Fixed Point value in Q32 format.
    /// </para>
    /// </summary>
    /// <remarks>
    /// http://x86asm.net/articles/fixed-point-arithmetic-and-tricks/
    /// </remarks>
    public struct Fixed32
    {
        /// <summary>
        /// The one
        /// </summary>
        public static readonly Fixed32 One = new Fixed32((long)1 << 32);
        
        /// <summary>
        /// The zero
        /// </summary>
        public static readonly Fixed32 Zero = new Fixed32(0);

        /// <summary>
        /// The epsilon value
        /// </summary>
        public static readonly Fixed32 Epsilon = new Fixed32(1);

        /// <summary>
        /// The maximum value
        /// </summary>
        public static readonly Fixed32 Maximum = new Fixed32(0x7FFFFFFFFFFFFFFF);

        /// <summary>
        /// The maximum value
        /// </summary>
        public static readonly Fixed32 Minimum = new Fixed32(~0x7FFFFFFFFFFFFFFF);

        /// <summary>
        /// Gets the Q.
        /// </summary>
        /// <value>The Q.</value>
        public int Q { get { return 32; }}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Fixed32.</returns>
        public static Fixed32 FromInteger(int value)
        {
            return new Fixed32((long)value << 32);
        }

        /// <summary>
        /// The value.
        /// </summary>
        public long Value;

        /// <summary>
        /// Gets as int32.
        /// </summary>
        /// <value>As int32.</value>
        public int ToInt32 { get { return (int) (Value >> 32); }}

        /// <summary>
        /// Gets as double.
        /// </summary>
        /// <value>As double.</value>
        public double ToDouble { get { return (double)Value / ((long)1 << 32); } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Fixed32"/> struct.
        /// </summary>
        /// <param name="q">The q.</param>
        /// <param name="value">The value.</param>
        public Fixed32(long value)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Fixed32"/> struct.
        /// </summary>
        /// <param name="other">The other.</param>
        public Fixed32(Fixed32 other)
            : this(other.Value)
        {
        }

        /// <summary>
        /// Implements the +.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Fixed32 operator +(Fixed32 a, Fixed32 b)
        {
            return new Fixed32(a.Value + b.Value);
        }

        /// <summary>
        /// Implements the -.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Fixed32 operator -(Fixed32 a, Fixed32 b)
        {
            return new Fixed32(a.Value - b.Value);
        }

        /// <summary>
        /// Multiplies two 64bit fixed point numbers in Q32 format into one Int128 value
        /// </summary>
        /// <remarks>http://svn.gnucash.org/docs/HEAD/group__Math128.html#ga88ed8c48ea98cfe246ee12f16081f47e</remarks>
        [Pure]
        private static Int128 Multiply(Fixed32 a, Fixed32 b)
        {
            long avalue = a.Value;
            long bvalue = b.Value;

            Int128 prod;

            prod.IsNegative = false;
            if (0 > avalue)
            {
                prod.IsNegative = !prod.IsNegative;
                avalue = -avalue;
            }

            if (0 > bvalue)
            {
                prod.IsNegative = !prod.IsNegative;
                bvalue = -bvalue;
            }

            ulong ahi = (ulong)(avalue >> 32);
            ulong alo = (ulong)avalue - (ahi << 32);

            ulong bhi = (ulong)bvalue >> 32;
            ulong blo = (ulong)bvalue - (bhi << 32);

            ulong d = alo * blo;
            ulong d1 = d >> 32;
            ulong d0 = d - (d1 << 32);

            ulong e = alo * bhi;
            ulong e1 = e >> 32;
            ulong e0 = e - (e1 << 32);

            ulong f = ahi * blo;
            ulong f1 = f >> 32;
            ulong f0 = f - (f1 << 32);

            ulong g = ahi * bhi;
            ulong g1 = g >> 32;
            ulong g0 = g - (g1 << 32);

            ulong sum = d1 + e0 + f0;
            ulong carry = 0;
            /* Can't say 1<<32 cause cpp will goof it up; 1ULL<<32 might work */
            const ulong roll = (long)1 << 32;

            const ulong pmax = roll - 1;
            while (pmax < sum)
            {
                sum -= roll;
                carry++;
            }

            prod.Low = (long)(d0 + (sum << 32));
            prod.High = (long)(carry + e1 + f1 + g0 + (g1 << 32));
            
            return prod;
        }

        /// <summary>
        /// Implements the +.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Fixed32 operator *(Fixed32 a, Fixed32 b)
        {
            long avalue = a.Value;
            long bvalue = b.Value;

#if false
            var ahi = (int)(avalue >> 32);
            var alo = (int)(avalue & 0xFFFFFFFF);

            var bhi = (int)(bvalue >> 32);
            var blo = (int)(bvalue & 0xFFFFFFFF);

            var result = ((blo*alo) >> 32) + (blo*ahi) + (bhi*alo) + ((bhi*ahi) << 32);
            return new Fixed32(result);
#else

            Int128 prod = Multiply(a, b);

            // shift to correct Q
            long result = (prod.High & 0xFFFFFFFF) << 32 | ((prod.Low >> 32) & 0xFFFFFFFF);

            // apply rounding
            long value = prod.Low & ((long) 1 << 31) << 1;
            result += value;

            return new Fixed32(result);
#endif
        }
    }
}
