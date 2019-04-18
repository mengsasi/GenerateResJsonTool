using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace GenerateTool {

    class Program {

        public static bool needStop = false;

        static void Main( string[] args ) {

            Console.WriteLine( "请稍候，不要关闭程序..." );

            FolderGenerate.Init();

            //GenerateTool文件夹
            string generateToolFolder = Environment.CurrentDirectory;
            var configJsonText = FileUtil.Load( generateToolFolder + "\\config.json" );
            JToken configJToken = JToken.Parse( configJsonText );

            DirectoryInfo generateToolInfo = new DirectoryInfo( generateToolFolder );
            string root = generateToolInfo.Parent.Parent.FullName;//工程目录

            if( configJToken["root"] != null ) {
                root = configJToken["root"].Value<string>();//配置的root路径
            }
            string resourcePath = root + @"\Egret\resource";//resource路径
            resourcePath += "\\";

            string webResourcePath = resourcePath;
            if( configJToken["webResourcePath"] != null ) {
                webResourcePath = root + configJToken["webResourcePath"].Value<string>();
            }

            //if( configJToken["svnPath"] != null ) {
            //    SVNVersion.InitSVNClient( configJToken["svnPath"].Value<string>() );
            //}

            //生成本地default.res.json

            //不加版本号后缀
            VersionUtils.UseVersionSuffix = false;

            var generateLocal = configJToken["generatelocal"].Value<bool>();
            if( generateLocal ) {
                string defaultResJson = resourcePath + configJToken["defaultresjson"].Value<string>();
                GenerateUtil.GenerateLocal( configJToken, resourcePath, defaultResJson );
            }

            if( configJToken["uploading"] != null ) {
                var uploading = configJToken["uploading"].Value<bool>();
                UploadingTool.MainUploading.isUploading = uploading;
            }

            if( UploadingTool.MainUploading.isUploading ) {
                //生成改变的资源组
                UploadingTool.MainUploading.MainProgram( root, configJToken );
            }

            //生成网络res.json

            //加版本号后缀
            VersionUtils.UseVersionSuffix = true;

            var generateWeb = configJToken["generateWeb"].Value<bool>();
            if( generateWeb ) {
                string gangsterResJson = webResourcePath + configJToken["gangsterresjson"].Value<string>();
                GenerateUtil.GenerateWeb( configJToken, webResourcePath, gangsterResJson );
            }

            //上传ftp
            UploadingTool.FTPUtils.InitFTP( configJToken );
            UploadingTool.MainUploading.Uploading( resourcePath );

            //SVNVersion.DeinitSVNClient();

            if( needStop ) {
                Console.ReadKey();
            }
        }

    }

}