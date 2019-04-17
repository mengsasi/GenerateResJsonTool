﻿using Newtonsoft.Json.Linq;
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
            string resourcePath = root + @"\Egret\resource";//resource路径

            if( configJToken["resource"] != null ) {
                resourcePath = configJToken["resource"].Value<string>();//配置的resource路径
            }
            resourcePath += "\\";

            if( configJToken["svnPath"] != null ) {
                VersionUtils.InitSVNClient( configJToken["svnPath"].Value<string>() );
            }
            //不加后缀
            VersionUtils.UseVersionSuffix = false;

            var generateLocal = configJToken["generatelocal"].Value<bool>();
            if( generateLocal ) {
                string defaultResJson = resourcePath + configJToken["defaultresjson"].Value<string>();
                GenerateUtil.GenerateLocal( configJToken, resourcePath, defaultResJson );
            }
            //加版本号
            VersionUtils.UseVersionSuffix = true;

            var generateWeb = configJToken["generateWeb"].Value<bool>();
            if( generateWeb ) {
                string gangsterResJson = resourcePath + configJToken["gangsterresjson"].Value<string>();
                GenerateUtil.GenerateWeb( configJToken, resourcePath, gangsterResJson );
            }

            VersionUtils.DeinitSVNClient();

            if( needStop ) {
                Console.ReadKey();
            }
        }

    }

}