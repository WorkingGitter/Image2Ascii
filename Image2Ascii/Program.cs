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
    class Program
    {   
        static void Main(string[] args)
        {
            // Check for the required number of parameters
            if(args.Length < 1)
            {
                DisplayHelp();
                return;
            }

            // Parse command line options
            String filename = "";
            ASCIIRenderer.RenderTypeEnum rt = ASCIIRenderer.RenderTypeEnum.rtSimple;
            foreach(var s in args)
            {
                if(String.Compare(s, "-Ts", true) == 0)
                {
                    rt = ASCIIRenderer.RenderTypeEnum.rtSimple;
                    continue;
                }

                if(String.Compare(s.Substring(0, 2), "-I", true) == 0)
                {
                    filename = s.Substring(2, s.Length - 2);
                    continue;
                }
            }

            if (String.IsNullOrEmpty(filename))
            {
                DisplayHelp();
                return;
            }

            // Render to screen
            ASCIIRenderer aRender = new ASCIIRenderer(filename, rt);
            aRender.RenderToConsole();
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  AArt -I<image filename> [-T<rendertype>]");
            Console.WriteLine();
            Console.WriteLine("options:");
            Console.WriteLine(" -Ts Simple conversion");
            return;
        }
    } // class Program



    /**************************************************************************
    * Main class that handles the image conversion 
    ***************************************************************************/
    public class ASCIIRenderer
    {
        public enum RenderTypeEnum
        {
            rtSimple
        }

        private string _imagepath = "";
        private RenderTypeEnum RenderType { get; set; } = RenderTypeEnum.rtSimple;

        public ASCIIRenderer(string imagepath, RenderTypeEnum rt)
        {
            _imagepath = imagepath;
            RenderType = rt;
        }


        /**************************************************************************
        * Does our conversion 
        ***************************************************************************/
        public void RenderToConsole()
        {
            switch(RenderType)
            {
                case RenderTypeEnum.rtSimple:
                default:
                    ConvertUsingSimpleMethod(_imagepath);
                    break;
            }
        }


        /**************************************************************************
        * Simple method of image conversion.
        * 
        * This uses a collection of characters that represents various
        * brightness levels.
        * 
        * These characters were choosen arbitrarily by sight only.
        * The scaling is done by sampling the equivalent position in the image.
        * An output size of 80 characters is used.
        * A vertical adjustment of 2.2 is used so that the final output looks
        * proportional.
        ***************************************************************************/
        private void ConvertUsingSimpleMethod(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                return;

            char[] charpix = { ' ', '.','-', ';', 'o','?', 'b','*','%','X', '#', '@' };

            try
            {
                using (Bitmap bitmap = new Bitmap(filename))
                {
                    // Calculate the scaling adjustment we will use. 
                    // Note that we are only doing this for the width. The height will be automatically
                    // done.
                    double scale = bitmap.Width / 80;
                    if (scale < 1.0f)
                        scale = 1.0f;

                    int y = 0;      // image Y-axis to sample
                    int x = 0;      // image X-axis to sample
                    int yC = 0;     // current console row
                    int xC = 0;     // current console column

                    while (y < bitmap.Height)
                    {
                        for (xC = 0; xC < 80; xC++)
                        {
                            // calculate the image X-axis to sample.
                            // We are doing a straight scaling.
                            x = (int)Math.Round(xC * scale, MidpointRounding.AwayFromZero);

                            // Adjust if we fall outside the image boundaries
                            if (x < 0)
                                x = 0;

                            if (x >= bitmap.Width)
                                x = bitmap.Width - 1;

                            Color pixel = bitmap.GetPixel(x, y);

                            // Get the brightness of this pixel.
                            // This ranges from 0.0 to 1.0
                            float brightness = pixel.GetBrightness();

                            // map this to our available character lookup table
                            int index = (int)Math.Round((brightness * (charpix.Length - 1)), MidpointRounding.AwayFromZero);
                            Debug.Assert(index >= 0);
                            Debug.Assert(index < charpix.Length);

                            // Write our character to screen
                            Console.Write(charpix[index]);
                        }
                        Console.WriteLine();

                        // increment row, then map to the equivalent Y-axis in our image
                        // Note, I am ajusting the ratio due to the console font used.
                        // A factor of 2.2 seem to produce the best result to elimitate 
                        // image distortion.
                        yC++;
                        y = (int)Math.Round(yC * scale * 2.2, MidpointRounding.AwayFromZero);
                    }

                } // using (Bitmap bitmap = new Bitmap(filename))

            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Unable to open file");
            }
            catch(System.ArgumentException)
            {
                Console.WriteLine("Invalid image input");
            }
        } // private void ConvertUsingSimpleMethod(string filename)

    }




}
