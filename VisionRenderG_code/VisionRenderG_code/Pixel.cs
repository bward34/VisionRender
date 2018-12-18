using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VisionRenderG_code
{
    class Pixel
    {
        public char Z { get; set; }
        public char radius { get; set; }
        public UInt16 angle { get; set; }
        public UInt16 redVal { get; set; }
        public UInt16 greenVal { get; set; }
        public UInt16 blueVal { get; set; }
        public string adjustedX { get; set; }
        public string adjustedY { get; set; }
        public string angleNonBinary { get; set; }
        public string nonBinaryZ { get; set; }
        public string nonBinaryRadius { get; set; }


        public void addZdata(char zVal)
        {
            nonBinaryZ = Convert.ToString(zVal);
            Z = zVal;
            //Z = Z.PadLeft(8, '0');
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

            // calculatedAngle = calculatedAngle * (divisions / 360);

            angle = (UInt16) Math.Round(calculatedAngle);
            angleNonBinary = Convert.ToString(angle);
            //angle = Convert.ToString(roundedAngle, 2);

            //angle = angle.PadLeft(8, '0');
        }

        public void addRadius(double X, double Y)
        {
            double scaleFactor = 0.8;
            double calculatedRadius = Math.Sqrt((X * X) + (Y * Y));

            while(calculatedRadius > 32)
            {
                calculatedRadius = calculatedRadius * scaleFactor;
            }

            radius = (char) Math.Round(calculatedRadius);
            nonBinaryRadius = Convert.ToString(radius);
            //radius = Convert.ToString(roundedRadius, 2);
            //radius = radius.PadLeft(8, '0');
        }

        public void addColor(UInt16 redVal, UInt16 blueVal, UInt16 greenVal)
        {
            this.redVal = redVal;
            this.blueVal = blueVal;
            this.greenVal = greenVal;
        }
    }

    class Coord
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
