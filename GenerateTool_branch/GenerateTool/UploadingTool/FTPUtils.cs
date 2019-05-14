using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace GenerateTool.UploadingTool {

    class FTPUtils {

        public static string UserName = "";
        public static string Password = "";

        public static string ProxyHost = "";
        public static int ProxyPort;

        public static bool UseProxy = false;

        public static string WebResourceUploadingDir = "";

        public static void InitFTP( JToken configJtoken ) {
            UserName = configJtoken["username"].Value<string>();
            Password = configJtoken["password"].Value<string>();

            MainUploading.FTPRootPath = configJtoken["ftpRootPath"].Value<string>();
            MainUploading.OuterNetFTPRootPath = configJtoken["ftpOuterNetRootPath"].Value<string>();

            //代理
            FTPUtils.ProxyHost = configJtoken["proxyHost"].Value<string>();
            FTPUtils.ProxyPort = configJtoken["proxyPort"].Value<int>();

            FTPUtils.WebResourceUploadingDir = configJtoken["webResourceUploadingDir"].Value<string>();
        }

        public static void UploadingFile( string filePath, string ftpPath ) {
            var url = ftpPath;
            FileInfo fileInfo = new FileInfo( filePath );
            var username = FTPUtils.UserName;
            var password = FTPUtils.Password;

            FTPUtils.CheckDir( ftpPath );

            FtpWebRequest reqFtp = FTPUtils.GetRequest( url, WebRequestMethods.Ftp.UploadFile, username, password );
            reqFtp.ContentLength = fileInfo.Length;

            //缓冲大小设置为2KB
            const int BufferSize = 2048;
            byte[] content = new byte[BufferSize];
            int contentLen;
            bool success = true;
            using( FileStream fs = fileInfo.OpenRead() ) {
                try {
                    using( Stream rs = reqFtp.GetRequestStream() ) {
                        contentLen = fs.Read( content, 0, BufferSize );
                        while( contentLen != 0 ) {
                            rs.Write( content, 0, contentLen );
                            contentLen = fs.Read( content, 0, BufferSize );
                        }
                    }
                }
                catch( Exception ex ) {
                    success = false;
                    Console.WriteLine( ex.Message );
                }
                finally {
                    Console.WriteLine( fileInfo.FullName + ( success ? " 上传成功" : " 上传失败" ) );
                    reqFtp = null;
                }
            }
        }

        private static FtpWebRequest GetRequest( string url, string method, string username = "", string password = "" ) {
            //根据服务器信息FtpWebRequest创建类的对象
            FtpWebRequest result = (FtpWebRequest)FtpWebRequest.Create( new Uri( url ) );
            //提供身份验证信息
            //result.Credentials = new NetworkCredential( username, password );
            //设置请求完成之后是否保持到FTP服务器的控制连接，默认值为true
            result.KeepAlive = false;
            //设置FTP命令 设置所要执行的FTP命令，
            result.Method = method;
            result.UseBinary = true;
            result.UsePassive = false;//表示连接类型为主动模式
            if( FTPUtils.UseProxy ) {
                WebProxy proxy = new WebProxy( FTPUtils.ProxyHost, FTPUtils.ProxyPort );
                result.Proxy = proxy;
            }
            return result;
        }

        //ftp://192.168.19.58/WebResource/resource  /assets/test/111.png
        public static void CheckDir( string filePath ) {
            var ftpRoot = MainUploading.FTPRootPath;
            var fileUrl = filePath.Replace( ftpRoot, "" );
            var dirs = fileUrl.Split( '/' );
            var dirPath = ftpRoot;
            for( int i = 0; i < dirs.Length - 1; i++ ) {
                if( dirs[i] == "" ) {
                    FTPUtils.MakeDir( dirPath );
                }
                else {
                    dirPath = dirPath + "/" + dirs[i];
                    FTPUtils.MakeDir( dirPath );
                }
            }
        }

        public static void MakeDir( string dirPath ) {
            if( !FTPUtils.FtpDirExists( dirPath ) ) {
                try {
                    FtpWebRequest ftp = FTPUtils.GetRequest( dirPath, WebRequestMethods.Ftp.MakeDirectory );
                    FtpWebResponse response = (FtpWebResponse)ftp.GetResponse();
                    response.Close();
                }
                catch( Exception ex ) {
                    Console.WriteLine( "创建文件失败，原因: " + ex.Message );
                }
            }
        }

        public static bool FtpDirExists( string dirPath ) {
            FtpWebRequest reqFtp = FTPUtils.GetRequest( dirPath, WebRequestMethods.Ftp.ListDirectory );
            FtpWebResponse resFtp = null;
            try {
                resFtp = (FtpWebResponse)reqFtp.GetResponse();
                FtpStatusCode code = resFtp.StatusCode;//OpeningData
                resFtp.Close();
                return true;
            }
            catch {
                if( resFtp != null ) {
                    resFtp.Close();
                }
                return false;
            }
        }

    }

}