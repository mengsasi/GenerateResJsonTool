using System.IO;
using System.Text;

namespace GenerateTool {

    class FileUtil {
        //载入文件
        public static string Load( string path ) {
            bool hasFile = File.Exists( path );
            if( !hasFile ) {
                return "";
            }
            string content;
            using( var fs = new FileStream( path, FileMode.Open, FileAccess.Read ) ) {
                using( var sr = new StreamReader( fs ) ) {
                    content = sr.ReadToEnd();
                }
            }
            return content;
        }

        public static void Save( string path, string content ) {
            File.WriteAllText( path, content, Encoding.UTF8 );
        }
    }
}
