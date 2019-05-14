using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace GenerateTool.UploadingTool {

    class MainUploading {

        public static string FTPRootPath = "";
        public static string OuterNetFTPRootPath = "";

        public static void MainUpload() {
            if( AssetOperation.isGenerate ) {
                MainUploading.Uploading();
                MainUploading.UploadingOuterNet();
            }
        }

        public static void Uploading() {
            if( AssetOperation.isUploading ) {
                FTPUtils.UseProxy = false;
                string root = Program.ProjectRootPath;
                string gangsterRes000 = Program.VersionFolder;
                var resourcePath = root + "\\" + gangsterRes000 + "\\";

                Program.ConsoleLog( "上传内网中..." );

                //上传gangsterRes001
                if( Program.Gangster_0_0_0_Path != "" ) {
                    FTPUtils.UploadingFile( Program.Gangster_0_0_0_Path, MainUploading.FTPRootPath + "/" + Program.Gangster0_0_0_res_json );
                }

                var list = AssetOperation.ListChangeFils;
                for( int i = 0; i < list.Count; i++ ) {
                    var path = list[i];
                    path = path.Replace( '\\', '/' );
                    var relatePath = AssetOperation.GetRelatePath( path, root );
                    var targetFtpPath = MainUploading.FTPRootPath + relatePath;
                    FTPUtils.UploadingFile( path, targetFtpPath );
                }
                Program.ConsoleLog( "上传内网完成" );
            }
        }

        public static void UploadingOuterNet() {
            if( Program.LockOuterNet ) {
                return;
            }
            if( AssetOperation.isUploadingOuterNet ) {
                FTPUtils.UseProxy = true;
                string root = Program.ProjectRootPath;
                string gangsterRes000 = Program.VersionFolder;
                var resourcePath = root + "\\" + gangsterRes000 + "\\";

                Program.ConsoleLog( "上传修改的资源到外网中..." );

                if( Program.Gangster_0_0_0_Path != "" ) {
                    FTPUtils.UploadingFile( Program.Gangster_0_0_0_Path, MainUploading.OuterNetFTPRootPath + "/" + Program.Gangster0_0_0_res_json );
                }

                var list = AssetOperation.ListChangeFils;
                for( int i = 0; i < list.Count; i++ ) {
                    var path = list[i];
                    path = path.Replace( '\\', '/' );
                    var relatePath = AssetOperation.GetRelatePath( path, root );
                    var targetFtpPath = MainUploading.OuterNetFTPRootPath + relatePath;
                    FTPUtils.UploadingFile( path, targetFtpPath );
                }
                Program.ConsoleLog( "上传修改的资源外网完成" );
                MainUploading.UploadingWebsources();
            }
        }

        public static void UploadingWebsources() {
            FTPUtils.UseProxy = true;
            string gangsterRes000 = Program.VersionFolder;

            Program.ConsoleLog( "上传webresources到外网中..." );

            string webresourcePath = Program.WebResourcePath + FTPUtils.WebResourceUploadingDir;
            var files = Directory.GetFiles( webresourcePath, "*.*", SearchOption.AllDirectories );
            foreach( var file in files ) {
                var path = file;
                path = path.Replace( '\\', '/' );
                var relatePath = AssetOperation.GetRelatePath( path, webresourcePath );
                var targetFtpPath = MainUploading.OuterNetFTPRootPath + "/" + gangsterRes000 + "/" + FTPUtils.WebResourceUploadingDir + relatePath;
                FTPUtils.UploadingFile( path, targetFtpPath );
            }
            Program.ConsoleLog( "上传webresources外网完成" );
        }

    }
}
