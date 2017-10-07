/*************************************************************************
 * Author: Richard Chin
 * Date: October 2017
 * Description:
 *      ASCII art generator using a given bitmap as source.
 *************************************************************************/


using System;

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
        }

    }
}
