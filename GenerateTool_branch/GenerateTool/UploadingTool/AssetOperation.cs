using Force.Crc32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace GenerateTool.UploadingTool {

    class AssetOperation {

        public static bool isGenerate = false;//生成修改的资源
        public static bool isUploading = false;//上传内网ftp
        public static bool isUploadingOuterNet = false;//上传外网ftp

        public static List<Group> ListChangeGroup = new List<Group>();

        public static void MainOperation( string rootPath, JToken configJToken ) {
            string root = rootPath;//工程目录
            root += "\\";
            //记录修改的文件的txt
            if( configJToken["versionTextPath"] == null ) {
                Program.ConsoleLog( "缺少版本文件地址" );
                return;
            }
            string versionText = root + configJToken["versionTextPath"].Value<string>();
            if( !File.Exists( versionText ) ) {
                Program.ConsoleLog( "不存在文件 " + versionText );
                return;
            }
            //删除之前生成的gangsterRes000文件夹
            string gangsterRes000 = Program.VersionFolder;
            var resourcePath = root + gangsterRes000;
            if( Directory.Exists( resourcePath ) ) {
                Directory.Delete( resourcePath, true );
            }

            //读取之前所有文件信息
            FileCollector.GenerateResourceInfo( versionText );

            var useInitTxt = configJToken["useInitTxt"].Value<bool>();
            if( useInitTxt ) {
                AssetOperation.GetChangeFiles();
            }
            else {
                //将更改的资源复制到新文件夹
                AssetOperation.FindChangeFiles( configJToken );
            }

            //生成资源组，添加到gangster.res.json中
            AssetOperation.GenerateChangeGroup( configJToken );
        }

        public static void GenerateChangeGroup( JToken configJToken ) {
            //路径
            string root = Program.ProjectRootPath;
            string gangsterRes000 = Program.VersionFolder;
            var resourcePath = root + "\\" + gangsterRes000 + "\\";
            List<Group> listGroup = new List<Group>();
            GenerateUtil.GenerateConfig( listGroup, configJToken["assetsGroups"], resourcePath );
            AssetOperation.ListChangeGroup = listGroup;
        }


        #region 查找更改的资源，复制到gangsterRes001文件夹中

        public static void FindChangeFiles( JToken configJToken ) {
            Console.WriteLine( "查找更改的资源中..." );
            string resourcePath = Program.ResourcePath;//root + @"\Egret\resource\"
            AssetOperation.ListChangeFils = new List<string>();
            foreach( var child in configJToken["assetsGroups"] ) {
                var confFolderPath = child["folderPath"].Value<string>();
                //排序WebAvatar文件夹
                if( confFolderPath.Contains( "Web" ) || confFolderPath.Contains( "web" ) ) {
                    continue;
                }
                //某个文件夹是后加到本地的，就是想打到包里，就不添加到网上更改的列表
                //只能是强更的时候，强更，本地才有这些资源
                if( child["ignoreChange"] != null ) {
                    if( child["ignoreChange"].Value<bool>() ) {
                        continue;
                    }
                }
                AssetOperation.FindChange( resourcePath, confFolderPath );
            }
            //config文件夹
            AssetOperation.FindChange( resourcePath, "config" );
            //Font文件夹
            AssetOperation.FindChange( resourcePath, "Font" );
        }

        //singles/temp/sound
        private static void FindChange( string resourcePath, string configPath ) {
            var resOldDict = FileCollector.ResourceDict;
            string assetPath = resourcePath + configPath;
            var files = Directory.GetFiles( assetPath, "*.*", SearchOption.AllDirectories );
            foreach( var file in files ) {
                FileStream stream = new FileStream( file, FileMode.Open );
                byte[] buffer = new byte[stream.Length];
                stream.Read( buffer, 0, (int)stream.Length );
                stream.Close();
                uint crc32 = Crc32Algorithm.Compute( buffer );

                //resource下相对路径 assets/singles/avatar/11.png
                var fileRelatePath = AssetOperation.GetRelatePath( file, resourcePath );
                if( resOldDict.ContainsKey( fileRelatePath ) ) {
                    if( resOldDict[fileRelatePath] == crc32 ) {
                        continue;
                    }
                }
                //更改的资源，或者新资源
                //复制到新路径下，等待上传
                AssetOperation.CopyToNewPath( file, fileRelatePath );

                //test
                //if( resOldDict.ContainsKey( fileRelatePath ) ) {
                //    if( resOldDict[fileRelatePath] != crc32 ) {
                //        AssetOperation.CopyToNewPath( file, fileRelatePath );
                //    }
                //}
            }
        }

        //靠init.txt生成的，知道更改的资源
        private static void GetChangeFiles() {
            string resourcePath = Program.ResourcePath;//root + @"\Egret\resource\"
            var resOldDict = FileCollector.ResourceDict;
            foreach( var item in resOldDict ) {
                string assetPath = resourcePath + item.Key;
                if( File.Exists( assetPath ) ) {
                    var fileRelatePath = AssetOperation.GetRelatePath( assetPath, resourcePath );
                    AssetOperation.CopyToNewPath( assetPath, fileRelatePath );
                }
                else {
                    Program.ConsoleLog( assetPath + " 不存在" );
                }
            }
        }

        public static List<string> ListChangeFils = new List<string>();

        private static void CopyToNewPath( string sourcePath, string targetRelate ) {
            string root = Program.ProjectRootPath;
            string gangsterRes000 = Program.VersionFolder;
            var targetPath = root + "\\" + gangsterRes000 + "\\" + targetRelate;
            AssetOperation.MakeDir( targetPath );
            File.Copy( sourcePath, targetPath, true );
            AssetOperation.ListChangeFils.Add( targetPath );
        }

        private static void MakeDir( string filePath ) {
            var root = Program.ProjectRootPath.Replace( '\\', '/' );
            string path = filePath.Replace( '\\', '/' );
            var fileRelatePath = AssetOperation.GetRelatePath( filePath, root );

            var dirs = fileRelatePath.Split( '/' );
            var dirPath = root;
            for( int i = 0; i < dirs.Length - 1; i++ ) {
                string dir = dirs[i];
                if( dir == "" ) {
                    if( !Directory.Exists( dirPath ) ) {
                        Directory.CreateDirectory( dirPath );
                    }
                }
                else {
                    dirPath = dirPath + "/" + dir;
                    if( !Directory.Exists( dirPath ) ) {
                        Directory.CreateDirectory( dirPath );
                    }
                }
            }
        }

        public static string GetRelatePath( string filePath, string rootPath ) {
            var fileRelatePath = filePath.Substring( rootPath.Length );
            fileRelatePath = fileRelatePath.Replace( '\\', '/' );
            return fileRelatePath;
        }

        #endregion 


    }
}
