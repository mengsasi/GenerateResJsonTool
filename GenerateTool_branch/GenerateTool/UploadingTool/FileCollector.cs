using System.Collections.Generic;
using System.IO;

namespace GenerateTool.UploadingTool {

    class FileCollector {

        public static Dictionary<string, uint> ResourceDict = new Dictionary<string, uint>();

        public static void GenerateResourceInfo( string filePath ) {
            FileCollector.ResourceDict = new Dictionary<string, uint>();
            FileStream fs = new FileStream( filePath, FileMode.Open );
            StreamReader sr = new StreamReader( fs );
            string line = "";
            while( ( line = sr.ReadLine() ) != null ) {
                if( line.Contains( ".exml" ) ) {
                    continue;
                }
                var info = new ResourceInfo( line );
                FileCollector.ResourceDict.Add( info.filePath, uint.Parse( info.version ) );

                //移除重命名检测
                var exist = FolderGenerate.globalRes.Find( item => item.Url == info.filePath );
                if( exist != null ) {
                    FolderGenerate.globalRes.Remove( exist );
                }
            }
            fs.Close();
            sr.Close();
        }

    }

    class ResourceInfo {

        public ResourceInfo( string line ) {
            var parts = line.Split( ':' );
            filePath = parts[0];
            version = parts[1];
        }

        public string filePath;
        public string version;

    }
}
