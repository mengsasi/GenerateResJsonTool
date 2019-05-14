using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateTool {

    class Helper {

        public static string GetUrl( string urlRoot, string path ) {
            if( urlRoot == "" ) {
                return path;
            }
            else {
                return urlRoot + "/" + path;
            }
        }

    }
}
