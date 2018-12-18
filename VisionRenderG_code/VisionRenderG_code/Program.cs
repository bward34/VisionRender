using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace VisionRenderG_code
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            String fileName = "C:\\Users\\TaySm\\CS 4710\\cube.gcode";
            String line;
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader(fileName);
                Dictionary<int, List<Pixel>> objectData = new Dictionary<int, List<Pixel>>();
                Dictionary<int, List<Coord>> coordinate_Data = new Dictionary<int, List<Coord>>();
                List<Pixel> currData = new List<Pixel>();
                List<Coord> line_data = new List<Coord>();
                String z_axis = @"([Z][0-9]*\.)";
                String x_y_axis = @"([X][0-9]*\.)";
                //Read the first line of text
                line = sr.ReadLine();
                int zLayer = 0;
                int X = 0;
                int Y = 0;
                int X2 = 0;
                int Y2 = 0;
                int coord_Count = 0;

                //Continue to read until you reach end of file
                while (line != null)
                {
                    Pixel Data = new Pixel();
                    Match z_Result = Regex.Match(line, z_axis);
                    int currZ = zLayer;
                    if (z_Result.Success)
                    {
                        zLayer++;
                        Console.WriteLine("zLayer: " + zLayer);
                    }

                    if (currZ < zLayer && zLayer > 1)
                    {
                        coordinate_Data.Add(currZ - 1, line_data);
                        line_data = new List<Coord>();
                        coord_Count = 0;
                    }

                    Match x_yResult = Regex.Match(line, x_y_axis);
                    if (x_yResult.Success && zLayer > 1)
                    {
                        bool isXdata = false;
                        bool isYdata = false;
                        string Xdata = "";
                        string Ydata = "";

                        foreach (char c in line)
                        {
                            if (isXdata && c == ' ')
                            {
                                isXdata = false;
                            }
                            if (isYdata && c == ' ')
                            {
                                isYdata = false;
                            }
                            if (isXdata)
                            {
                                Xdata += c;
                            }
                            if (isYdata)
                            {
                                Ydata += c;
                            }
                            if (c == 'X')
                            {
                                isXdata = true;
                            }
                            if (c == 'Y')
                            {
                                isYdata = true;
                            }
                        }

                        Console.WriteLine(Xdata + " " + Ydata);
                        if (coord_Count == 0)
                        {
                            double x, y;
                            Double.TryParse(Xdata, out x);
                            Double.TryParse(Ydata, out y);
                            X = (int) x;
                            Y = (int) y;
                            coord_Count++;
                        }
                        else if (coord_Count == 1)
                        {
                            double x2, y2;
                            Double.TryParse(Xdata, out x2);
                            Double.TryParse(Ydata, out y2);
                            X2 = (int) x2;
                            Y2 = (int) y2;
                            Console.WriteLine("X,Y: " + X + " " + Y + "\n");
                            Console.WriteLine("X2,Y2: " + X2 + " " + Y2 + "\n");
                            DrawLine(coordinate_Data, X, Y, X2, Y2, currZ - 1, line_data);
                            X = X2;
                            Y = Y2;
                        }
                    }
                    //Read the next line
                    line = sr.ReadLine();
                }
                coordinate_Data.Add(zLayer - 1, line_data);
                //close the file
                sr.Close();

                AddDataToDictionary(coordinate_Data, objectData);
                //Example #4: Append new text to an existing file.
                // Write to a file
                using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(@"C:\Users\TaySm\CS 4710\Cube_test.txt", true))
                {
                    file.WriteLine("1");
                    file.WriteLine(pixelCount.ToString().PadLeft(5,'0'));
                    foreach (int key in objectData.Keys)
                    {
                        List<Pixel> listOfPixels = objectData[key];
                        foreach(Pixel currPixel in listOfPixels)
                        {
                            file.WriteLine(((int)currPixel.Z).ToString().PadLeft(2,'0') + " " + ((int)currPixel.radius).ToString().PadLeft(2, '0') + " " + currPixel.angle.ToString().PadLeft(3, '0') + " " + currPixel.redVal.ToString().PadLeft(5, '0') + " " + currPixel.greenVal.ToString().PadLeft(5, '0') + " " + currPixel.blueVal.ToString().PadLeft(5, '0'));
                        }
                    }


                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }


        }

        private static byte[] convertToBytes (UInt32 value)
        {
            byte[] intBytes = BitConverter.GetBytes(value);
            Array.Reverse(intBytes);
            return intBytes;
        }

        private static byte[] convertToBytes(UInt16 value)
        {
            byte[] intBytes = BitConverter.GetBytes(value);
            Array.Reverse(intBytes);
            return intBytes;
        }

        private static byte[] convertToBytes(char value)
        {
            byte[] intBytes = BitConverter.GetBytes(value);
            Array.Reverse(intBytes);
            return intBytes;
        }

        private static byte[] convertToBytes(Pixel currPixel)
        {
            byte[] intBytes = new byte[10];
            Array.Reverse(intBytes);
            return intBytes;
        }


        private static UInt32 pixelCount;
        private static void AddDataToDictionary(Dictionary<int, List<Coord>> coordinate_Data, Dictionary<int, List<Pixel>> objectData)
        {
            pixelCount = 0;
            foreach(int key in coordinate_Data.Keys)
            {
                if (key != 0)
                { 
                    List<Pixel> currPixelsLayer = new List<Pixel>();
                    foreach (Coord coordinate in coordinate_Data[key])
                    {
                        Pixel currPixel = new Pixel();
                        double xData = coordinate.X;
                        double yData = coordinate.Y;
                        currPixel.addAngle(xData, yData, 360);
                        currPixel.addRadius(xData, yData);
                        currPixel.addZdata((char)(key-1));
                        currPixel.addColor(0xFFFF, 0xFFFF, 0xFFFF);
                        Console.WriteLine("Z layer:" + (int)currPixel.Z + "\n");
                        Console.WriteLine("Angle: " + currPixel.angle + "\n");
                        Console.WriteLine("Radius: " + (int)currPixel.radius + "\n");
                        currPixelsLayer.Add(currPixel);
                        pixelCount++;
                    }
                    objectData.Add(key-1, currPixelsLayer);
                    Console.WriteLine("\n");
                }
            }
        }

        private static List<Coord> DrawLine(this Dictionary<int, List<Coord>> coordinates, int x0, int y0, int x1, int y1, int z, List<Coord> line_data)
        {
            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2, e2;
            for (; ; )
            {
                Coord curr_coord = new Coord();
                curr_coord.X = x0;
                curr_coord.Y = y0;
                Console.WriteLine("line data: " + x0 + " " + y0 + "\n");
                line_data.Add(curr_coord);
                if (x0 == x1 && y0 == y1) break;
                e2 = err;
                if (e2 > -dx) { err -= dy; x0 += sx; }
                if (e2 < dy) { err += dx; y0 += sy; }
            }
            return line_data;
        }
    }
}
