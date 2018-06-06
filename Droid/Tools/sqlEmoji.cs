using System;
using System.IO;
using Xamarin.Forms;
using X.Droid.Tools;
using X.Helpers;

[assembly: Dependency(typeof(sqlEmoji))]
namespace X.Droid.Tools
{
    public class sqlEmoji : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(path, filename);
        }
    }
}
