using ImageMagick;
using System;
using System.Windows;

namespace GE_Utilities
{
    public class GeTif
    {

        //--- GetSizeMM -------------------------------------
        // get size in mm
        static public Size GetSizeMM(string path)
        {
            Size size =new Size();
            using (MagickImageCollection tiffPageCollection = new MagickImageCollection())
            {
                try
                {
                    tiffPageCollection.Read(path);
                    foreach (MagickImage tiffPage in tiffPageCollection)
                    {
                        Density d = tiffPage.Density;
                        size.Height = tiffPage.Height*25.4/d.Y;
                        size.Width  = tiffPage.Width*25.4/d.X;
                    }
                }
                catch (Exception)
                { }
            }
            return size;
        }

        //--- GetSizePX -------------------------------------
        // get size in pixels
        static public Size GetSizePX(string path)
        {
            Size size =new Size(); 
            using (MagickImageCollection tiffPageCollection = new MagickImageCollection())
            {
                tiffPageCollection.Read(path);
                foreach (MagickImage tiffPage in tiffPageCollection)
                {
                    size.Height = tiffPage.Height;
                    size.Width  = tiffPage.Width;
                }
            }
            return size;
        }
    }
}
