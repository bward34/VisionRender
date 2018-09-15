using System;

namespace VisionRender
{
    class DegreeFinder
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Enter x cordinant:");
            Int32.TryParse(Console.ReadLine(), out int x);
            Console.WriteLine("Enter y cordinant:");
            Int32.TryParse(Console.ReadLine(), out int y);

            double degrees = Math.Atan2(y , x) * (180 / Math.PI);
            Console.WriteLine($"Degrees is {degrees}");

        }
    }
}
