using System;
using System.IO;

namespace Zapp.Assets
{
    public static class AssetsHelper
    {
        public static Stream Read(string file)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", file);

            return File.OpenRead(path);
        }
    }
}
