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
using System.Collections.Generic;
using System.Linq;

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
            long rt = 0;
            foreach(var s in args)
            {
                if(String.Compare(s, "-Ts", true) == 0)
                {
                    rt |= (long)ASCIIRenderer.RenderOptions.rtAlgSimple;
                    continue;
                }

                if(String.Compare(s, "-D", true) == 0)
                {
                    rt |= (long)ASCIIRenderer.RenderOptions.rtShowWtdChrs;
                    continue;
                }

                if (String.Compare(s, "-g", true) == 0)
                {
                    rt |= (long)ASCIIRenderer.RenderOptions.rtWtdChrs;
                    continue;
                }

                if (String.Compare(s.Substring(0, 2), "-I", true) == 0)
                {
                    filename = s.Substring(2, s.Length - 2);
                    continue;
                }
            }

            if (rt == 0)
                rt = (long)ASCIIRenderer.RenderOptions.rtAlgSimple;

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
            Console.WriteLine("  AArt -I<image filename> [options]");
            Console.WriteLine();
            Console.WriteLine("options:");
            Console.WriteLine(" -Ts Simple conversion (simple char set)");
            Console.WriteLine(" -g  Generate and use weighted characters");
            Console.WriteLine(" -d  Display weighted characters");
            return;
        }
    } // class Program



    /**************************************************************************
    * Main class that handles the image conversion 
    ***************************************************************************/
    public class ASCIIRenderer
    {
        public enum RenderOptions
        {
            rtAlgSimple        = 1,
            rtShowWtdChrs   = 1 << 1,
            rtWtdChrs  = 1 << 2
            

        }

        private string _imagepath = "";
        private long _options = (long)RenderOptions.rtAlgSimple;
        private List<char> _CharTable = new List<char>()
                                                { ' ', '.', '-', ';', 'o', '?', 'b', '*', '%', 'X', '#', '@' };

        public ASCIIRenderer(string imagepath, long options)
        {
            _imagepath = imagepath;
            _options = options;
        }


        /**************************************************************************
        * Does our conversion 
        ***************************************************************************/
        public void RenderToConsole()
        {
            if( (_options & (long)RenderOptions.rtShowWtdChrs) > 0 )
            {
                // show simple source
                Console.WriteLine();
                Console.Write("Simple: ");
                foreach (var c in _CharTable)
                    Console.Write(c);

                BuildCharacterWeighting(ref _CharTable);
                Console.WriteLine();
                Console.Write("Calculated: ");
                foreach (var c in _CharTable)
                    Console.Write(c);

                return;
            }

            // Build characters to be used to render image
            if ((_options & (long)RenderOptions.rtWtdChrs) > 0)
            {
                BuildCharacterWeighting(ref _CharTable);
            }

            if ((_options & (long)RenderOptions.rtAlgSimple) > 0)
            {
                ConvertUsingSimpleMethod(_imagepath);
                return;
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
                            int index = (int)Math.Round((brightness * (_CharTable.Count - 1)), MidpointRounding.AwayFromZero);
                            Debug.Assert(index >= 0);
                            Debug.Assert(index < _CharTable.Count);

                            // Write our character to screen
                            Console.Write(_CharTable[index]);
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



        /**************************************************************************
        * This will dynamically build a character weighting table for use when
        * Rendering images.
        * 
        * 
        ***************************************************************************/
        private void BuildCharacterWeighting(ref List<char> CharTable)
        {
            CharTable.Clear();

            int w = 20;
            int h = 20;

            Dictionary<char, int> table = new Dictionary<char, int>();
            Bitmap bitmap = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(bitmap);
            Font f = new Font("Consolas", 12, FontStyle.Regular, GraphicsUnit.Pixel);

            // Loop through all the usuable character range
            for (int i = 33; i < 127; i++)
            {
                g.Clear(Color.White);

                string s = new string((char)i, 1);
                g.DrawString(s, f, new SolidBrush(Color.Black), 0, 0);

                int hit = 0;
                for (int j = 0; j < h; j++)
                {
                    for (int k = 0; k < w; k++)
                    {
                        Color c = bitmap.GetPixel(k, j);
                        if (c.GetBrightness() < 0.4)
                            hit++;
                    }
                }

                table[(char)i] = hit;

                SizeF sz = g.MeasureString(s, f);
                Debug.WriteLine($"{s} : width: {sz.Width}, height: {sz.Height}");
            }

            var myList = table.ToList();
            myList.Sort((x, y) => x.Value.CompareTo(y.Value));
            foreach (var v in myList)
                CharTable.Add(v.Key);

            f.Dispose();
            g.Dispose();
            bitmap.Dispose();   // release resources
        }

    } // public class ASCIIRenderer




}
