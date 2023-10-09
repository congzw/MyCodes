using System;

namespace CountSize
{
    static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("{0}: {1} byte(s) scope:[{2}-{3}]",
                typeof(byte).Name.PadLeft(8), sizeof(byte).NumberPad(2),
                byte.MinValue.NumberPad(32, true), byte.MaxValue.NumberPad(32));
            Console.WriteLine("{0}: {1} byte(s) scope:[{2}-{3}]",
                typeof(sbyte).Name.PadLeft(8), sizeof(sbyte).NumberPad(2),
                sbyte.MinValue.NumberPad(32, true), sbyte.MaxValue.NumberPad(32));
            Console.WriteLine("{0}: {1} byte(s) scope:[{2}-{3}]",
                typeof(short).Name.PadLeft(8), sizeof(short).NumberPad(2),
                short.MinValue.NumberPad(32, true), short.MaxValue.NumberPad(32));
            Console.WriteLine("{0}: {1} byte(s) scope:[{2}-{3}]",
                typeof(ushort).Name.PadLeft(8), sizeof(ushort).NumberPad(2),
                ushort.MinValue.NumberPad(32, true), ushort.MaxValue.NumberPad(32));
            Console.WriteLine("{0}: {1} byte(s) scope:[{2}-{3}]",
                typeof(int).Name.PadLeft(8), sizeof(int).NumberPad(2),
                int.MinValue.NumberPad(32, true), int.MaxValue.NumberPad(32));
            Console.WriteLine("{0}: {1} byte(s) scope:[{2}-{3}]",
                typeof(uint).Name.PadLeft(8), sizeof(uint).NumberPad(2),
                uint.MinValue.NumberPad(32, true), uint.MaxValue.NumberPad(32));
            Console.WriteLine("{0}: {1} byte(s) scope:[{2}-{3}]",
                typeof(long).Name.PadLeft(8), sizeof(long).NumberPad(2),
                long.MinValue.NumberPad(32, true), long.MaxValue.NumberPad(32));
            Console.WriteLine("{0}: {1} byte(s) scope:[{2}-{3}]",
                typeof(ulong).Name.PadLeft(8), sizeof(ulong).NumberPad(2),
                ulong.MinValue.NumberPad(32, true), ulong.MaxValue.NumberPad(32));
            Console.WriteLine("{0}: {1} byte(s) scope:[{2}-{3}]",
                typeof(float).Name.PadLeft(8), sizeof(float).NumberPad(2),
                float.MinValue.NumberPad(32, true), float.MaxValue.NumberPad(32));
            Console.WriteLine("{0}: {1} byte(s) scope:[{2}-{3}]",
                typeof(double).Name.PadLeft(8), sizeof(double).NumberPad(2),
                double.MinValue.NumberPad(32, true), double.MaxValue.NumberPad(32));
            Console.WriteLine("{0}: {1} byte(s) scope:[{2}-{3}]",
                typeof(decimal).Name.PadLeft(8), sizeof(decimal).NumberPad(2),
                decimal.MinValue.NumberPad(32, true), decimal.MaxValue.NumberPad(32));
            Console.WriteLine("{0}: {1} byte(s)",
                typeof(bool).Name.PadLeft(8), sizeof(bool).NumberPad(2));
            Console.WriteLine("{0}: {1} byte(s)",
                typeof(char).Name.PadLeft(8), sizeof(char).NumberPad(2));
            Console.WriteLine("{0}: {1} byte(s) ",
                typeof(IntPtr).Name.PadLeft(8), IntPtr.Size.NumberPad(2));
            Console.ReadLine();
        }

        public static string NumberPad<T>(this T value, int length, bool right = false)
        {
            if (right)
            {
                return value.ToString().PadRight(length);
            }
            else
            {
                return value.ToString().PadLeft(length);
            }
        }
    }
}