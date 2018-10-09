using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace VisionRenderG_code
{
    class Program
    {
        static void Main(string[] args)
        {
            String fileName = "C:\\Users\\TaySm\\Desktop\\BlenderObjects\\redBlueCube.gcode";
            String line;
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader(fileName);

                //Read the first line of text
                line = sr.ReadLine();
                String z_axis = @"([Z][0-9]*\.)";
                String x_y_axis = @"([X][0-9]*\.)";
                bool isZUpdated = false;
                int zLayer = 0;
                int index = 0;
                Dictionary<int,List<Pixel>> objectData = new Dictionary<int,List<Pixel>>();
                List<Pixel> currData = new List<Pixel>();

                //Continue to read until you reach end of file
                while (line != null)
                {
                    
                    Pixel Data = new Pixel();
                    Match z_Result = Regex.Match(line, z_axis);
                    int currZ = zLayer;
                    if (z_Result.Success) {
                        
                        zLayer++;
                        Data.addZdata(zLayer);
                        Console.WriteLine(zLayer.ToString());

                    }

                    if(currZ < zLayer)
                    {
                        objectData.Add(currZ, currData);
                        currData = new List<Pixel>();
                    }
                    
                    Match x_yResult = Regex.Match(line, x_y_axis);
                    if (x_yResult.Success)
                    {
                        bool isXdata = false;
                        bool isYdata = false;
                        string Xdata = "";
                        string Ydata = "";
                        
                        foreach(char c in line)
                        {
                            if(isXdata && c == ' ')
                            {
                                isXdata = false;
                            }
                            if(isYdata && c == ' ')
                            {
                                isYdata = false;
                            }
                            if(isXdata)
                            {
                                Xdata += c;
                            }
                            if(isYdata)
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

                        double X;
                        Double.TryParse(Xdata, out X);
                        double Y;
                        Double.TryParse(Xdata, out Y);

                        Data.addAngle(X, Y, 360);
                        Data.addRadius(X, Y);

                        Console.WriteLine("Angle: " + Data.angle + " Radius: " + Data.radius);
                        

                        currData.Add(Data);
                    }

                    
                    //Read the next line
                    line = sr.ReadLine();
                }

                //close the file
                sr.Close();
                Console.ReadLine();
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
    }
}
