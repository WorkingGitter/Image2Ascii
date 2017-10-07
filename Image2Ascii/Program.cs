/*************************************************************************
 * Author: Richard Chin
 * Date: October 2017
 * Description:
 *      ASCII art generator using a given bitmap as source.
 *************************************************************************/


using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace Image2Ascii
{
    public static class AAOptions
    {
    }

    class Program
    {   
        // >Image2Ascii <bitmapfilename>
        static void Main(string[] args)
        {

            if(args.Length < 1)
            {
                Console.WriteLine("Please supply the bitmap file path");
                return;
            }

            ConvertBitmapToASCII(args[0]);

        }

        static void ConvertBitmapToASCII(string filename)
        {
            char[] charpix = { ' ', '.', ';', 'o', 'b', 'X', '#', '@' };

            // Open bitmap
            try
            {
                using (Bitmap bitmap = new Bitmap(filename))
                {
                    double scale = bitmap.Width / 80;
                    if (scale < 1.0f)
                        scale = 1.0f;

                    int y = 0;
                    int x = 0;
                    int yC = 0;
                    int xC = 0;

                    while(y < bitmap.Height)
                    {
                        for(xC = 0; xC < 80; xC++)
                        {
                            Color pixel = bitmap.GetPixel(x, y);

                            // Calculate the brightness of this pixel.
                            // This ranges from 0.0 to 1.0
                            float brightness = pixel.GetBrightness();

                            // map this to our available lookup table
                            int index = (int)Math.Round((brightness * charpix.Length) / 1.0, MidpointRounding.AwayFromZero);
                            Debug.Assert(index >= 0);
                            Debug.Assert(index < charpix.Length);

                            // Write our character to screen
                            Console.Write(charpix[index]);

                            x = (int)Math.Round(xC * scale, MidpointRounding.AwayFromZero);
                            if (x < 0)
                                x = 0;
                            if (x >= bitmap.Width)
                                x = bitmap.Width - 1;
                        }
                        Console.WriteLine();

                        yC++;
                        y = (int)Math.Round(yC * scale * 2.2, MidpointRounding.AwayFromZero);
                    }


                } // using (Bitmap bitmap = new Bitmap(filename))

            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Unable to open file");
                return;
            }
        }

    }
}
