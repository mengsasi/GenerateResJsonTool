using SharpSvn;
using SharpSvn.Remote;
using System;

namespace GenerateTool {

    class SVNVersion {

        private static SvnClient client;
        private static string svnPath = "";
        private static long latestRevision = 0;

        public static void InitSVNClient( string path ) {
            svnPath = path;
            client = new SvnClient();

            SvnRemoteSession remoteSession = new SvnRemoteSession( new Uri( svnPath ) );
            remoteSession.GetLatestRevision( out latestRevision );
        }

        public static void DeinitSVNClient() {
            if( client != null ) {
                client.Dispose();
            }
        }

        public static bool UseVersionSuffix = false;

        public static string GetSVNVersion( string filePath ) {
            if( SVNVersion.svnPath == null || SVNVersion.svnPath == "" ) {
                return "";
            }
            string ver = "";
            if( UseVersionSuffix ) {
                string path = SVNVersion.svnPath + "/" + filePath;
                SvnInfoEventArgs clientInfo;
                SvnPathTarget target = new SvnPathTarget( filePath );
                try {
                    client.GetInfo( target, out clientInfo );
                    ver = "?v=" + clientInfo.LastChangeRevision;
                }
                catch {
                    //还没有提交到svn的文件，使用svn最新版本号
                    ver = "?v=" + latestRevision;
                }
            }
            return ver;
        }
    }
}
