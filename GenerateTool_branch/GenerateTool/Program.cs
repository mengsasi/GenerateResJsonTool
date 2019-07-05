using GenerateTool.UploadingTool;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace GenerateTool {

    class Program {

        public static bool needStop = false;
        public static bool IgnoreWeb = false;
        public static string Version = "";
        public static string VersionFolder = "";
        public static string ProjectRootPath = "";//工程目录
        public static string ResourcePath = "";
        public static string WebResourcePath = "";
        public static string Gangster_0_0_0_Path = "";
        public static string Gangster0_0_0_res_json = "";
        public static bool LockOuterNet = true;//锁定外网，不往外网传东西

        public static bool AddVersion = false;

        static void Main(string[] args) {

            Console.WriteLine("请稍候，不要关闭程序...");


            #region 读取配置

            string curFolder = Environment.CurrentDirectory;
            var configJsonText = FileUtil.Load(curFolder + "\\config.json");
            JToken configJToken = JToken.Parse(configJsonText);

            DirectoryInfo curDirInfo = new DirectoryInfo(curFolder);
            string root = curDirInfo.Parent.FullName;//工程目录
            if(configJToken["root"] != null) {
                root = configJToken["root"].Value<string>();//配置的root路径
            }
            Program.ProjectRootPath = root;

            string resourcePath = root + @"\Egret\resource";//resource路径
            resourcePath += "\\";
            Program.ResourcePath = resourcePath;

            string webResourcePath = root;
            if(configJToken["webResourcePath"] != null) {
                webResourcePath += configJToken["webResourcePath"].Value<string>();
                webResourcePath += "\\";
            }
            Program.WebResourcePath = webResourcePath;
            //是否忽略本地中的WebAvatar文件夹
            if(configJToken["ignoreWeb"] != null) {
                Program.IgnoreWeb = configJToken["ignoreWeb"].Value<bool>();
            }
            string version = "";
            if(configJToken["version"] != null) {
                version = configJToken["version"].Value<string>();
            }
            Program.Version = version;//版本号
            string confVersionFolder = configJToken["versionFolder"].Value<string>();//版本文件夹名gangsterRes
            Program.VersionFolder = confVersionFolder + Program.Version.Replace(".", "");//gangsterRes001

            if(configJToken["generate"] != null) {
                AssetOperation.isGenerate = configJToken["generate"].Value<bool>(); ;
            }
            if(configJToken["uploading"] != null) {
                AssetOperation.isUploading = configJToken["uploading"].Value<bool>();
            }
            if(configJToken["uploadingOuterNet"] != null) {
                AssetOperation.isUploadingOuterNet = configJToken["uploadingOuterNet"].Value<bool>();
            }

            Program.LockOuterNet = configJToken["lockOuterNet"].Value<bool>();

            #endregion


            #region 生成本地default.res.json

            Program.AddVersion = false;

            //不加版本号后缀
            VersionUtils.UseVersionSuffix = false;
            var generateLocal = configJToken["generatelocal"].Value<bool>();

            bool tempIgnoreWeb = Program.IgnoreWeb;
            Program.IgnoreWeb = true;
            //先在Tools文件夹生成一个
            FolderGenerate.Init();
            if(generateLocal) {
                string tooldefaultResJson = curFolder + "\\" + configJToken["defaultresjson"].Value<string>();
                GenerateUtil.GenerateLocal(configJToken, resourcePath, tooldefaultResJson);
            }

            Program.IgnoreWeb = tempIgnoreWeb;
            //生成本地使用的
            FolderGenerate.Init();
            if(generateLocal) {
                string defaultResJson = resourcePath + configJToken["defaultresjson"].Value<string>();
                GenerateUtil.GenerateLocal(configJToken, resourcePath, defaultResJson);
            }

            #endregion


            #region 更改的资源

            //Program.IgnoreWeb = true;//不忽略，就是生成网络资源
            //Program.AddVersion = true;
            //VersionUtils.UseVersionSuffix = true;//加版本号后缀
            //FolderGenerate.Init();

            //if( AssetOperation.isGenerate ) {
            //    AssetOperation.MainOperation( root, configJToken );
            //}

            #endregion


            #region 生成patch，将更改的文件复制到根目录一个文件夹

            //是否生成patch更改的资源文件夹
            var IsGeneratePathDir = configJToken["generatePathDir"].Value<bool>();
            if(IsGeneratePathDir) {

                PatchTool.PatchOperation.MainPatch(configJToken);

            }

            #endregion 


            #region 生成网络res.json

            var generateWeb = configJToken["generateWeb"].Value<bool>();
            if(generateWeb) {
                string resName = configJToken["gangsterresjson"].Value<string>();
                resName = resName.Replace("[version]", Program.Version);
                string gangsterResJson = webResourcePath + resName;
                GenerateUtil.GenerateWeb(configJToken, webResourcePath, gangsterResJson);
                Program.Gangster_0_0_0_Path = gangsterResJson;
                Program.Gangster0_0_0_res_json = resName;
            }

            #endregion


            #region 上传ftp

            FTPUtils.InitFTP(configJToken);
            MainUploading.MainUpload();

            #endregion 


            if(needStop) {
                Console.ReadKey();
            }
        }

        //遍历时，web开头文件夹忽略continue
        public static bool CheckWebFolder(string path) {
            if(Program.IgnoreWeb) {
                if(path.StartsWith("web") || path.StartsWith("Web")) {
                    return true;
                }
            }
            return false;
        }

        public static void ConsoleLog(string log) {
            Console.WriteLine(log);
            Program.needStop = true;
        }

    }

}