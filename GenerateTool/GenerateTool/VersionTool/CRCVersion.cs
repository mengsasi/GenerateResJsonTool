using Force.Crc32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateTool {

    class CRCVersion {

        public static string GetCRCVersion( string filePath ) {
            //var inputArray = new byte[realDataLength + 4];
            //// write real data to inputArray
            //Crc32Algorithm.ComputeAndWriteToEnd( inputArray ); // last 4 bytes contains CRC
            //                                                   // transferring data or writing reading, and checking as final operation
            //if( !Crc32Algorithm.IsValidWithCrcAtEnd( inputArray ) )
            //    throw new InvalidOperationException( "Data was tampered" );

            byte[] zipdata = File.ReadAllBytes( filePath );
            uint crc = Crc32CAlgorithm.Compute( zipdata );
            return "?v=" + crc;
        }

    }
}
