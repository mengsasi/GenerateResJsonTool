using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateTool {

    class VersionUtils {

        public static bool UseVersionSuffix = false;

        public static string GetCRC32() {
            string crc = "";
            if( UseVersionSuffix ) {







                crc = "?v=";
            }
            return crc;
        }

    }
}
