using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VisionRenderG_code
{
    class Pixel
    {
        public string Z { get; set; }
        public string angle { get; set; }
        public string radius { get; set; }
        public string redVal { get; set; }
        public string greenVal { get; set; }
        public string blueVal { get; set; }
        public string adjustedX { get; set; }
        public string adjustedY { get; set; }
        public string angleNonBinary { get; set; }
        public string nonBinaryZ { get; set; }
        public string nonBinaryRadius { get; set; }


        public void addZdata(int zVal)
        {
            nonBinaryZ = Convert.ToString(zVal);
            Z = Convert.ToString(zVal, 2);
            Z = Z.PadLeft(8, '0');
        }

        public void addAngle(double X, double Y, int divisions)
        {
            double calculatedAngle = Math.Atan(Y/ X) * (180 / Math.PI); //Find the angle in degrees

            if(divisions == 100)
            {
                calculatedAngle = calculatedAngle / 3.6;
            }
            if(divisions == 200)
            {
                calculatedAngle = calculatedAngle / 1.8;
            }

            int roundedAngle = (int) Math.Round(calculatedAngle);
            angleNonBinary = Convert.ToString(roundedAngle);
            angle = Convert.ToString(roundedAngle, 2);

            angle = angle.PadLeft(8, '0');
        }

        public void addRadius(double X, double Y)
        {
            double scaleFactor = 0.8;
            double calculatedRadius = Math.Sqrt((X * X) + (Y * Y));

            while(calculatedRadius > 32)
            {
                calculatedRadius = calculatedRadius * scaleFactor;
            }

            int roundedRadius = (int) Math.Round(calculatedRadius);
            nonBinaryRadius = Convert.ToString(roundedRadius);
            radius = Convert.ToString(roundedRadius, 2);
            radius = radius.PadLeft(8, '0');
        }

        void addColor(int redVal, int blueVal, int greenVal)
        {

        }
    }

    class Coord
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}
