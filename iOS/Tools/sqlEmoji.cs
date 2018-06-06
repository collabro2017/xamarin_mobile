using System;
using System.IO;
using Xamarin.Forms;
using X.Helpers;
using X.iOS.Tools;

[assembly: Dependency(typeof(sqlEmoji))]
namespace X.iOS.Tools
{

    public class sqlEmoji : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libFolder = Path.Combine(docFolder, "..", "Library", "Databases");

            if (!Directory.Exists(libFolder))
            {
                Directory.CreateDirectory(libFolder);
            }

            return Path.Combine(libFolder, filename);
        }

         
    }
}

