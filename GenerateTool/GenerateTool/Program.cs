using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace GenerateTool {

    class Program {

        public static bool needStop = false;
        public static bool IgnoreWeb = false;
        public static string Version = "";
        public static string VersionFolder = "";

        static void Main( string[] args ) {

            Console.WriteLine( "请稍候，不要关闭程序..." );

            //GenerateTool文件夹
            string generateToolFolder = Environment.CurrentDirectory;
            var configJsonText = FileUtil.Load( generateToolFolder + "\\config.json" );
            JToken configJToken = JToken.Parse( configJsonText );

            DirectoryInfo generateToolInfo = new DirectoryInfo( generateToolFolder );
            string root = generateToolInfo.Parent.Parent.FullName;//工程目录


            #region 读取配置

            if( configJToken["root"] != null ) {
                root = configJToken["root"].Value<string>();//配置的root路径
            }
            string resourcePath = root + @"\Egret\resource";//resource路径
            resourcePath += "\\";

            string webResourcePath = resourcePath;
            if( configJToken["webResourcePath"] != null ) {
                webResourcePath = root + configJToken["webResourcePath"].Value<string>();
                webResourcePath += "\\";
            }
            if( configJToken["ignoreWeb"] != null ) {
                Program.IgnoreWeb = configJToken["ignoreWeb"].Value<bool>();
            }
            string version = "";
            if( configJToken["version"] != null ) {
                version = configJToken["version"].Value<string>();
            }
            Program.Version = version.Replace( ".", "" );
            Program.VersionFolder = configJToken["versionFolder"].Value<string>();

            //if( configJToken["svnPath"] != null ) {
            //    SVNVersion.InitSVNClient( configJToken["svnPath"].Value<string>() );
            //}

            #endregion


            #region 生成本地default.res.json

            //不加版本号后缀
            VersionUtils.UseVersionSuffix = false;
            FolderGenerate.Init();
            var generateLocal = configJToken["generatelocal"].Value<bool>();
            if( generateLocal ) {
                string defaultResJson = resourcePath + configJToken["defaultresjson"].Value<string>();
                GenerateUtil.GenerateLocal( configJToken, resourcePath, defaultResJson );
            }

            #endregion


            #region 记录更改待上传的资源

            if( configJToken["uploading"] != null ) {
                var uploading = configJToken["uploading"].Value<bool>();
                UploadingTool.MainUploading.isUploading = uploading;
            }
            if( UploadingTool.MainUploading.isUploading ) {
                //生成改变的资源组
                UploadingTool.MainUploading.MainProgram( root, configJToken );
            }

            #endregion 


            #region 生成网络res.json

            Program.IgnoreWeb = false;//不忽略，就是生成网络资源

            //加版本号后缀
            VersionUtils.UseVersionSuffix = true;
            FolderGenerate.Init();
            var generateWeb = configJToken["generateWeb"].Value<bool>();
            if( generateWeb ) {
                string resName = configJToken["gangsterresjson"].Value<string>();
                resName = resName.Replace( "[version]", version );
                string gangsterResJson = webResourcePath + resName;
                GenerateUtil.GenerateWeb( configJToken, webResourcePath, gangsterResJson );
            }

            #endregion


            #region 上传ftp

            UploadingTool.FTPUtils.InitFTP( configJToken );
            UploadingTool.MainUploading.Uploading( resourcePath );

            #endregion 


            //SVNVersion.DeinitSVNClient();

            if( needStop ) {
                Console.ReadKey();
            }
        }

        //遍历时，web开头文件夹忽略continue
        public static bool CheckWebFolder( string path ) {
            if( Program.IgnoreWeb ) {
                if( path.StartsWith( "web" ) || path.StartsWith( "Web" ) ) {
                    return true;
                }
            }
            return false;
        }

    }

}