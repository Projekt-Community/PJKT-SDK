using System;
using System.IO;
using System.IO.Compression;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJKT.SDK2
{
    public class PjktFileExporter //: IDisposable
    {
#if UNITY_EDITOR
        
        //[MenuItem("PJKT/TestExporter")]
        public static void Test()
        {
            if(Selection.activeObject is GameObject)
            {
                GameObject booth = Selection.activeObject as GameObject;
                PjktFileExporter exporter = new PjktFileExporter("test");
                exporter.CreateBoothfile(booth);
            }
        }

        public readonly string TempDirectory;
        public readonly string CommunityName;
        
        public PjktFileExporter(string  communityName)
        {
            CommunityName = communityName;
            TempDirectory = Path.GetTempPath() + "PjktSdk\\" + communityName;
        }
        
        public string CreateBoothfile(GameObject booth)
        {
            if (booth == null)
            {
                Debug.LogError("Cannot create booth file from null object");
                return string.Empty;
            }
            
            //create prefab of the booth first
            string boothName = CommunityName + "_" + booth.name;
            string path = "Assets/PjktTemp/" + boothName + ".prefab";
            
            //check if the directory exists
            if (!Directory.Exists("Assets/PjktTemp"))
            {
                Directory.CreateDirectory("Assets/PjktTemp");
            }
            
            //create temp directories for the booth files
            CreateTempfolders(CommunityName);
            
            //create the prefab
            PrefabUtility.SaveAsPrefabAsset(booth, path);

            //get all dependedncies of the prefab
            string[] dependencies = AssetDatabase.GetDependencies(path);
            
            //sorts duplicated of the files into temp appdata folder
            SortFiles(dependencies);
            
            //do community and booth info json here
            
            //now zip it up
            string zipPath = "Assets/PjktTemp/" + boothName + ".zip"; //later change it to community name + event name
            ZipFile.CreateFromDirectory(TempDirectory, zipPath);
            
            //cleanup the prefab
            if (File.Exists(path)) File.Delete(path);

            if (File.Exists(zipPath)) return zipPath;
            return string.Empty;
        }
        
        //uses %localappdata%\Temp\PjktSdk\communityName to store the files
        private void CreateTempfolders(string CommunityName)
        {
            if (!Directory.Exists(TempDirectory))
            {
                Directory.CreateDirectory(TempDirectory);
            }
            else
            {
                //delete existing files
                DirectoryInfo di = new DirectoryInfo(TempDirectory);
                foreach (FileInfo file in di.GetFiles()) file.Delete();
            }
            
            //folders for textures, materials, models, etc
            CreateOrClearFolder("Textures");
            CreateOrClearFolder("Materials");
            CreateOrClearFolder("Models");
            CreateOrClearFolder("Animations");
            CreateOrClearFolder("Audio");
            CreateOrClearFolder("Shaders");
            CreateOrClearFolder("OtherFiles");
        }

        private void CreateOrClearFolder(string fileType)
        {
            //if directory already exists then delete files in it
            if (!Directory.Exists($"{TempDirectory}\\{fileType}"))
            {
                Directory.CreateDirectory($"{TempDirectory}\\{fileType}");
            }
            else
            {
                DirectoryInfo di = new DirectoryInfo($"{TempDirectory}\\{fileType}");
                foreach (FileInfo file in di.GetFiles()) file.Delete();
            }
        }
        
        //sorts the files into correct folders in the temp appdata folder
        private void SortFiles(string[] files)
        {
            //chat is dir real?
            if (!Directory.Exists(TempDirectory)) throw new Exception("Temp Directory does not exist");
            
            //for each file in the dependencies, get the file type and make a duplicate in the temp folder
            foreach (string file in files)
            {
                //if the file path starts with packages ignore it
                if (file.StartsWith("Packages")) continue;
                
                string fileType = GetFileType(file);
                string fileName = Path.GetFileName(file);
                string newFilePath = $"{TempDirectory}\\{fileType}\\{fileName}";
                
                //grab its .meta file as well
                string metaFile = file + ".meta";
                
                //copy the file to the new location
                File.Copy(file, newFilePath);
                File.Copy(metaFile, newFilePath + ".meta");
            }
        }

        //figures out what folder the file is supposed to go in
        private string GetFileType(string file)
        {
            //sort by file extensions, type causes too many issues
            string extension = Path.GetExtension(file);

            switch (extension)
            {
                //textures
                case ".png":
                    return "Textures";
                case ".jpg":
                    return "Textures";
                case ".exr":
                    return "Textures";
                case ".tif":
                    return "Textures";
                
                //animations
                case ".anim":
                    return "Animations";
                case ".controller":
                    return "Animations";
                
                //audio
                case ".wav":
                    return "Audio";
                case ".mp3":
                    return "Audio";
                case ".flac":
                    return "Audio";
                
                //materials
                case ".mat":
                    return "Materials";
                
                //shaders
                case ".shader":
                    return "Shaders";
                
                //models
                case ".fbx":
                    return "Models";
                case ".obj":
                    return "Models";
                
                //prefabs
                case ".prefab":
                    return "";
                
                //everything else
                default:
                    return "OtherFiles";
            }
        }

        public void Dispose()
        {
            //cleanup temp files
            if (Directory.Exists(TempDirectory))
            {
                Directory.Delete(TempDirectory, true);
            }

            if (Directory.Exists("Assets/PjktTemp/"))
            {
                Directory.Delete("Assets/PjktTemp/", true);
            }
        }
#endif
    }
}