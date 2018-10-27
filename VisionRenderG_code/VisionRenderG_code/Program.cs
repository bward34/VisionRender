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
            String fileName = "C:\\Users\\TaySm\\Desktop\\BlenderObjects\\redBlueCube.gcode";
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
                double X = 0;
                double Y = 0;
                double X2 = 0;
                double Y2 = 0;
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
                            //double x1, y1;
                            Double.TryParse(Xdata, out X);
                            Double.TryParse(Ydata, out Y);
                            //X = (int) Math.Round(x1);
                            //Y = (int) Math.Round(y1);
                            coord_Count++;
                        }
                        else if (coord_Count == 1)
                        {
                            //double x2, y2;
                            Double.TryParse(Xdata, out X2);
                            Double.TryParse(Ydata, out Y2);
                            //X2 = (int) Math.Round(x2);
                            //Y2 = (int) Math.Round(y2);
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

        private static void AddDataToDictionary(Dictionary<int, List<Coord>> coordinate_Data, Dictionary<int, List<Pixel>> objectData)
        {
            foreach(int key in coordinate_Data.Keys)
            {
                if (key != 0)
                {
                    List<Pixel> currPixelsLayer = new List<Pixel>();
                    Console.WriteLine("Z layer: " + key + "\n");
                    foreach (Coord coordinate in coordinate_Data[key])
                    {
                        Pixel currPixel = new Pixel();
                        double xData = coordinate.X;
                        double yData = coordinate.Y;
                        currPixel.addAngle(xData, yData, 360);
                        currPixel.addRadius(xData, yData);
                        currPixel.addZdata(key);
                        Console.WriteLine("Angle: " + currPixel.angle + "\n");
                        Console.WriteLine("Radius: " + currPixel.radius + "\n");
                        currPixelsLayer.Add(currPixel);
                    }
                    objectData.Add(key, currPixelsLayer);
                    Console.WriteLine("\n");
                }
            }
        }

        private static List<Coord> DrawLine(this Dictionary<int, List<Coord>> coordinates, double x0, double y0, double x1, double y1, int z, List<Coord> line_data)
        {
            int dx = Math.Abs((int)(x1 - x0)), sx = x0 < x1 ? 1 : -1;
            int dy = Math.Abs((int)(y1 - y0)), sy = y0 < y1 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2, e2;
            for (; ; )
            {
                double prevX;
                double prevY;
                Console.WriteLine("line data: " + x0 + " " + y0 + "\n");
                Coord curr_coord = new Coord();
                curr_coord.X = x0;
                curr_coord.Y = y0;
                prevX = x0;
                prevY = y0;
                line_data.Add(curr_coord);
                if (x0 == x1 && y0 == y1) break;
                e2 = err;
                if (e2 > -dx)
                {
                    if (sx == -1)
                    {
                        if ((x0 + sx) < x1)
                        {
                            Coord end_coord = new Coord();
                            end_coord.X = x1;
                            end_coord.Y = y1;
                            line_data.Add(end_coord);
                            break;
                        }
                    }
                    if (sx == 1)
                    {
                        if ((x0 + sx) > x1)
                        {
                            Coord end_coord = new Coord();
                            end_coord.X = x1;
                            end_coord.Y = y1;
                            line_data.Add(end_coord);
                            break;
                        }
                    }

                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dy)
                {
                    if (sy == 1)
                    {
                        if ((y0 + sy) > y1)
                        {
                            Coord end_coord = new Coord();
                            end_coord.X = x1;
                            end_coord.Y = y1;
                            line_data.Add(end_coord);
                            break;
                        }
                    }
                    if (sy == -1)
                    {
                        if ((y0 + sy) < x1)
                        {
                            Coord end_coord = new Coord();
                            end_coord.X = x1;
                            end_coord.Y = y1;
                            line_data.Add(end_coord);
                            break;
                        }
                    }

                    err += dx;
                    y0 += sy;
                }
                x0 = Math.Truncate(x0 * 1000) / 1000;
                y0 = Math.Truncate(y0 * 1000) / 1000;
                if(prevX == x0 && prevY == y0)
                {
                    Coord end_coord = new Coord();
                    end_coord.X = x1;
                    end_coord.Y = y1;
                    line_data.Add(end_coord);
                    break;
                }
            }
            
            return line_data;
        }
    }
}
