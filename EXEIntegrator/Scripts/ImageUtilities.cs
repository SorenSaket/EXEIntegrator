using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.IO;
namespace EXEIntegrator.Scripts
{
    public static class ImageUtilities
    {
        public static ImageSource ToImageSource(string path)
        {
            FileInfo file = new FileInfo(path);

            switch (file.Extension)
            {
                default:
                    break;
            }

            // If EXE



            return null;

        }
    }
}
