/////////////////////////////////////////////////////////
///                                                   ///
///    Written by Chanoler                            ///
///    If you are a VRChat employee please hire me    ///
///    chanolercreations@gmail.com                    ///
///                                                   ///
/////////////////////////////////////////////////////////

using System.IO;

namespace PJKT.SDK
{
    public static class PJKTUtil
    {
        //Recursively create the folders in which the file will be saved
        public static void CreatePath(string file)
        {
            if (Directory.Exists(file)) return;
            CreatePath(Path.GetDirectoryName(file));
            Directory.CreateDirectory(file);
        }

        public static void CreateDirectory(string file)
        {
            CreatePath(Path.GetDirectoryName(file));
        }
    }
}