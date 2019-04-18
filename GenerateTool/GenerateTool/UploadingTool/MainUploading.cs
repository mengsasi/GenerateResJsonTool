using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateTool.UploadingTool {

    class MainUploading {

        public static string FTPRootPath = "";

        public static bool isUploading = false;

        public static List<Group> ListChangeGroup = new List<Group>();

        public static void MainProgram( string rootPath, JToken configJToken ) {
            string root = rootPath;//工程目录

            if( configJToken["versionTextPath"] == null ) {
                Console.WriteLine( "缺少版本文件地址" );
                Console.ReadKey();
                return;
            }
            MainUploading.FTPRootPath = configJToken["ftpRootPath"].Value<string>();
            root += "\\";
            string versionText = root + configJToken["versionTextPath"].Value<string>();
            if( !File.Exists( versionText ) ) {
                Console.WriteLine( "不存在文件 " + versionText );
                Console.ReadKey();
                return;
            }

            //查找txt文件
            var allLocalRes = ResContainer.ListResLocalAll;

            var list = FileCollector.GenerateResourceInfo( versionText );

            MainUploading.ListChangeGroup = new List<Group>();

            //改变的文件列表
            foreach( var item in list ) {
                var url = item.filePath;
                var existRes = allLocalRes.Find( temp => temp.Url == url );
                if( existRes != null ) {
                    existRes.Url = existRes.Url + "?v=" + item.version;
                    var group = existRes.group;
                    var groupName = group.Name;
                    var existGroup = MainUploading.ListChangeGroup.Find( g => g.Name == groupName );
                    if( existGroup == null ) {
                        var newGroup = new Group();
                        newGroup.Name = groupName;
                        newGroup.listRes.Add( existRes );
                        MainUploading.ListChangeGroup.Add( newGroup );
                    }
                    else {
                        existGroup.listRes.Add( existRes );
                    }
                }
                else {
                    Console.WriteLine( "default.res.json 之前不存在 " + url );
                }
            }
            for( int i = 0; i < MainUploading.ListChangeGroup.Count; i++ ) {
                var g = MainUploading.ListChangeGroup[i];
                g.Keys = g.GenerateKey();
            }
        }

        public static void Uploading( string resourcePath ) {
            if( MainUploading.isUploading ) {
                Console.WriteLine( "上传中..." );
                var list = FileCollector.ListResourceInfo;

                for( int i = 0; i < list.Count; i++ ) {
                    var item = list[i];
                    var filePath = resourcePath + item.filePath;
                    var targetFtpPath = MainUploading.FTPRootPath + "/" + item.filePath;

                    FTPUtils.UploadingFile( filePath, targetFtpPath );
                }
            }
        }
    }
}
