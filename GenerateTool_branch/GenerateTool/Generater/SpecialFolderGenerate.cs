﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace GenerateTool {

    class SpecialFolderGenerate {


        #region config文件夹

        public static List<Res> GenerateConfig( string resourcePath ) {
            List<Res> list = new List<Res>();
            string folderPath = resourcePath + "config";
            DirectoryInfo configFolder = new DirectoryInfo( folderPath );
            FileInfo[] files = configFolder.GetFiles();
            foreach( var item in files ) {
                var name = item.Name.Split( '.' );
                var fileName = name[0];
                var ext = item.Extension;
                var extension = ext.ToLower();
                Res res = new Res();
                res.Url = "config/" + item.Name;
                res.Name = fileName + "_" + extension.Replace( ".", "" );
                if( extension == ".json" ) {
                    res.Type = "json";
                }
                else {
                    res.Type = "bin";
                }
                list.Add( res );
            }
            return list;
        }

        #endregion


        #region Font文件夹

        public static List<Res> GenerateFont( string resourcePath ) {
            List<Res> list = new List<Res>();
            string folderPath = resourcePath + "Font";
            DirectoryInfo fontFolder = new DirectoryInfo( folderPath );
            FileInfo[] files = fontFolder.GetFiles();
            foreach( var item in files ) {
                var name = item.Name.Split( '.' );
                string n = "";
                for( int i = 0; i < name.Length - 1; i++ ) {
                    n += name[i];
                    if( i < name.Length - 2 ) {
                        n += "_";
                    }
                }
                var ext = item.Extension;
                var extension = ext.ToLower();
                Res res = new Res();
                res.Url = "Font/" + item.Name;
                res.Name = n;
                res.Type = "bin";//font assetmanager会崩
                list.Add( res );
            }
            return list;
        }

        #endregion


        #region sheets文件夹

        //检查图集的大图和子图文件夹重复
        public static void CheckDelGroup( Group group, List<Group> listGroup ) {
            var listRes = group.listRes;
            for( int j = 0; j < listRes.Count; j++ ) {
                var resItem = listRes[j];
                if( resItem.Type == "sheet" ) {
                    var index = listGroup.FindIndex( n => n.Name == resItem.Name );
                    if( index != -1 ) {
                        Program.ConsoleLog( "待删除子图文件夹 " + resItem.Name );
                        listGroup.RemoveAt( index );
                    }
                }
            }
        }

        //temp sound那种文件夹，根据文件夹下子文件夹生成组，一个文件夹一组
        public static void SheetFolder2Groups( string folderPath, string urlRoot, List<Group> listGroup ) {
            if( Directory.Exists( folderPath ) ) {
                var allFolders = Directory.GetDirectories( folderPath, "*.*", SearchOption.TopDirectoryOnly );
                for( int i = 0; i < allFolders.Length; i++ ) {
                    var path = allFolders[i];
                    Group group = SpecialFolderGenerate.SheetFolder2Group( path, urlRoot );
                    if( group != null ) {
                        SpecialFolderGenerate.CheckDelGroup( group, listGroup );
                        listGroup.Add( group );
                    }
                }
            }
        }

        // folderPath .../assets/temp/avatar
        // urlRoot       assets/temp/(avatar)
        public static Group SheetFolder2Group( string folderPath, string urlRoot ) {
            if( !Directory.Exists( folderPath ) ) {
                return null;
            }
            DirectoryInfo folder = new DirectoryInfo( folderPath );
            string folderName = folder.Name;
            if( Program.CheckWebFolder( folderName ) ) {
                return null;
            }
            Group group = new Group();
            group.Name = folderName;//temp下子文件夹
            List<Res> list = new List<Res>();
            var url = Helper.GetUrl( urlRoot, folderName );
            SpecialFolderGenerate.SheetFolder2Reses( folderPath, url, list );
            group.listRes = list;
            group.Keys = group.GenerateKey();
            return group;
        }

        public static List<Res> SheetFolder2Reses( string folderPath, string urlRoot, List<Res> list ) {
            SpecialFolderGenerate.SheetFolder2Res( folderPath, urlRoot, list );
            var allFolders = Directory.GetDirectories( folderPath, "*.*", SearchOption.TopDirectoryOnly );
            for( int i = 0; i < allFolders.Length; i++ ) {
                var path = allFolders[i];
                DirectoryInfo folder = new DirectoryInfo( path );
                string folderName = folder.Name;
                if( Program.CheckWebFolder( folderName ) ) {
                    continue;
                }
                var url = Helper.GetUrl( urlRoot, folder.Name );
                SpecialFolderGenerate.SheetFolder2Reses( path, url, list );
            }
            return list;
        }

        public static void SheetFolder2Res( string folderPath, string urlRoot, List<Res> list ) {
            if( !Directory.Exists( folderPath ) ) {
                return;
            }
            DirectoryInfo folder = new DirectoryInfo( folderPath );
            FileInfo[] files = folder.GetFiles();
            foreach( var item in files ) {
                var ext = item.Extension;
                var extension = ext.ToLower();
                var name = item.Name.Split( '.' );
                var fileName = name[0];

                Res res = new Res();
                var url = Helper.GetUrl( urlRoot, item.Name + VersionUtils.GetVersion( item.FullName ) );
                res.Url = url;
                res.Name = fileName;

                if( extension == ".png" || extension == ".jpg" ) {
                    res.Type = "image";
                    var existImg = list.Find( n => n.Name == fileName && n.Type == "image" );
                    if( existImg != null ) {
                        Program.ConsoleLog( "存在重复文件名文件" + urlRoot + "/" + item.Name );
                    }
                    var sheetJson = list.Find( n => n.Name == fileName && n.Type == "json" );
                    list.Add( res );

                    if( sheetJson != null ) {
                        //sheet
                        sheetJson.Type = "sheet";
                        list.Remove( res );

                        var fullName = item.FullName.Replace( "png", "json" );
                        //subKeys
                        sheetJson.SubKeys = SpecialFolderGenerate.GenerateSubKeys( fullName );
                    }
                }
                else if( extension == ".json" ) {
                    res.Type = "json";
                    var sheetImg = list.Find( n => n.Name == fileName && n.Type == "image" );
                    list.Add( res );

                    if( sheetImg != null ) {
                        //sheet
                        res.Type = "sheet";
                        list.Remove( sheetImg );

                        //subKeys
                        res.SubKeys = SpecialFolderGenerate.GenerateSubKeys( item.FullName );
                    }
                }
            }
        }

        public static string GenerateSubKeys( string filePath ) {
            var subkeys = "";
            var fileStr = FileUtil.Load( filePath );
            JToken file = JToken.Parse( fileStr );
            var frames = file["frames"];
            var index = 0;
            foreach( JToken child in frames.Children() ) {
                if( index > 0 ) {
                    subkeys += ",";
                }
                var property = child as JProperty;
                subkeys += property.Name;
                index++;
            }
            return subkeys;
        }

        #endregion


    }
}
