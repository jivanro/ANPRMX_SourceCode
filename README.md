## Project Files
If you want to download a test executable or the Release Dll, go to [https://github.com/jivanro/ANPRMX_ExecutableFiles](https://github.com/jivanro/ANPRMX_ExecutableFiles)
## Project Description
ANPR MX is a simple Automatic Plate Recognition DLL for ordinary North American type plates. (It won't identify "European" type ones.)

It uses OpenCV for image processing and is capable of analyzing images with a maximum resolution of 800x600 or lower. ( *.jpg / *.png) file formats allowed.

The code is partially based on the JavaANPR project, which is owned by Ondrej Martinsky.
(See: http://javaanpr.sourceforge.net for details.)

However, the following things were modified:

* * Code entirely written in C# using VS 2010 and the .NET Framework 4.0
* * Uses OpenCV instead of the java.awt libraries.
* * Replaces the plate band finding algorithm with a different approach based on OpenCV Blobs.
* * Recognizes the North American average plate size only. __It won't work for the European type plates__.

The library is simple and ready to be used on commercial or open source projects as it is licensed under the new BSD format.

To make it easy for anyone to understand how the DLL works, I also provided an executable that shows its capabilities.
You can also download a series of random car photos to test it. (See the "Downloads" section.)

## IMPORTANT
Please make sure you agree with the following licenses before you proceed to use this software:

* * BSD (for OpenCV and ANPR MX)
* * LGPL 3 (for linking to OpenCVSharp)

In case of doubts or comments, please feel free to contact me at jivanro@hotmail.com

## Donate

ANPRMX is a free open source project that is developed in personal time. 

If you find it useful and want to show your support, ANY amount donated is deeply appreciated:
[![PayPal - The safer, easier way to donate online!](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&amp;hosted_button_id=YTJHTYGGVNPMQ)

**Executable Example:**
![Executable image](https://github.com/jivanro/ANPRMX_ExtraFiles/blob/master/Home_Example_Screenshot.jpg)
