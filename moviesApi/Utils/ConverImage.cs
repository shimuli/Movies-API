using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace moviesApi.Utils
{
    public  class ConverImage
    {
        public static Image ImageString()
        {
            //Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String("kkdjjdfjhdfdfdfdf");
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            //Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }
    }
}
