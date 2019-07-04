Image2Ascii
===========

[![Build Status](https://dev.azure.com/RichardChin/Image2Ascii/_apis/build/status/WorkGitter.Image2Ascii?branchName=master)](https://dev.azure.com/RichardChin/Image2Ascii/_build/latest?definitionId=2&branchName=master)

### About
Renders a given image to the command line console.
Images are redrawn using predefined ASCII characters.

### Overview
This simple project was created as a means of gaining more insight of the C# programming language.
This is a **console application**, running under **Windows** (_tested under Win7 and Win10_).

### Usage
Running the program is quite simple. It currently accepts has three parameters, two of which are optional.
Executing the program without any parameters will display the help screen.

```
Prompt:\>AsciiArt.exe
Usage:
  AArt -i<image filename> [options]

options:
 -i<image filename> path and filename to the image. Also accepts web links.
   (BMP, GIF, JPG, PNG or TIFF)
 -g  Generate and use weighted characters
 -d  Display weighted characters
```

**Examples**:

**Prompt:>** `AsciiArt.exe -ic:\temp\face.bmp`

**Prompt:>** `AsciiArt.exe -ihttps://wallpaperscraft.com/image/megan_fox_eyes_charm_shadow_face_25178_2560x1600.jpg`

By default, the application uses an arbitry, predefined set of characters.
Using the `-g` option, the application, uses an alternative algorithm to find the optimal characters that gives the 
best shades of light to dark.  

`-d` will display to the console, the characters used.

### Summary
Personally, I found that the default option produces _slightly_ better results.
With more time, it would be good to add some improvements such as:

- Better image sampling
- Algorithm that detects and use image edges.
