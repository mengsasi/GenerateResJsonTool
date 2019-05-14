using Force.Crc32;
using System.IO;

namespace GenerateTool {

    class CRCVersion {

        public static string GetCRCVersion( string filePath ) {
            byte[] buffer = File.ReadAllBytes( filePath );
            //FileStream stream = new FileStream( filePath, FileMode.Open );
            //byte[] buffer = new byte[stream.Length];
            //stream.Read( buffer, 0, (int)stream.Length );
            //stream.Close();
            uint crc32 = Crc32CAlgorithm.Compute( buffer );
            return "?v=" + crc32;
        }

    }
}
