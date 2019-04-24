using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateTool {

    class VersionUtils {

        public static bool UseVersionSuffix = false;

        public static string GetVersion( string filePath ) {
            string ver = "";
            if( UseVersionSuffix ) {
                ver = CRCVersion.GetCRCVersion( filePath );
            }
            return ver;
        }

    }
}
