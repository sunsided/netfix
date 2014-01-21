namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = Fixed32.FromInteger(2);
            var b = Fixed32.FromInteger(4096);
            var c = a + b;
            var d = c*a;
        }
    }
}
