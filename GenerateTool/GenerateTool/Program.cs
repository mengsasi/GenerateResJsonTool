using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GenerateTool {

    class Program {

        public static bool needStop = false;

        static void Main( string[] args ) {

            FolderGenerate.Init();

            //GenerateTool文件夹
            string generateToolFolder = Environment.CurrentDirectory;
            var configJsonText = FileUtil.Load( generateToolFolder + "\\config.json" );
            JToken configJToken = JToken.Parse( configJsonText );

            DirectoryInfo generateToolInfo = new DirectoryInfo( generateToolFolder );
            string root = generateToolInfo.Parent.Parent.FullName;//工程目录
            string resourcePath = root + @"\Egret\resource";//resource路径

            if( configJToken["resource"] != null ) {
                resourcePath = configJToken["resource"].Value<string>();//配置的resource路径
            }
            resourcePath += "\\";

            VersionUtils.UseVersionSuffix = false;

            var generateLocal = configJToken["generatelocal"].Value<bool>();
            if( generateLocal ) {
                string defaultResJson = resourcePath + configJToken["defaultresjson"].Value<string>();
                GenerateUtil.GenerateLocal( configJToken, resourcePath, defaultResJson );
            }

            VersionUtils.UseVersionSuffix = true;

            var generateWeb = configJToken["generateWeb"].Value<bool>();
            if( generateWeb ) {
                string gangsterResJson = resourcePath + configJToken["gangsterresjson"].Value<string>();
                GenerateUtil.GenerateWeb( configJToken, resourcePath, gangsterResJson );
            }

            if( needStop ) {
                Console.ReadKey();
            }
        }

    }

}