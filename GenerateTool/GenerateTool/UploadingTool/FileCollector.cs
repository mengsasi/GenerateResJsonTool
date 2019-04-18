using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateTool.UploadingTool {

    class FileCollector {

        public static List<ResourceInfo> ListResourceInfo = new List<ResourceInfo>();

        public static List<ResourceInfo> GenerateResourceInfo( string filePath ) {
            FileCollector.ListResourceInfo = new List<ResourceInfo>();
            FileStream fs = new FileStream( filePath, FileMode.Open );
            StreamReader sr = new StreamReader( fs );
            string line = "";
            while( ( line = sr.ReadLine() ) != null ) {
                if( line.Contains( ".exml" ) ) {
                    continue;
                }
                var info = new ResourceInfo( line );
                FileCollector.ListResourceInfo.Add( info );
            }
            fs.Close();
            sr.Close();
            return FileCollector.ListResourceInfo;
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
