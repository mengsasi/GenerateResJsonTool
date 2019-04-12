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

                var rd = new Random();

                var version = rd.Next().ToString();


                crc = "?v=" + version;
            }
            return crc;
        }

    }
}
