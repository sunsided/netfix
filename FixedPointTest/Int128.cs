namespace ConsoleApplication1
{
    /// <summary>
    /// Struct Int128
    /// </summary>
    internal struct Int128
    {
        /// <summary>
        /// The high part
        /// </summary>
        public long High;

        /// <summary>
        /// The low part
        /// </summary>
        public long Low;

        /// <summary>
        /// <code>true</code> if number is negative
        /// </summary>
        public bool IsNegative;

        /// <summary>
        /// <code>true</code> if number won't fit in signed 64-bit
        /// </summary>
        public bool IsBig
        {
            get { return (High != 0) || ((Low >> 63) != 0); }
        }
    }
}
