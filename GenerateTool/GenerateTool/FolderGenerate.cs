using System;
using System.Collections.Generic;
using System.IO;

namespace GenerateTool {

    class FolderGenerate {

        //全局所有资源，用于判断是否重名
        public static List<Res> globalRes = new List<Res>();

        public static void Init() {
            globalRes = new List<Res>();
        }

        public static void CheckRepeat( Res res ) {
            var exist = globalRes.Find( n => n.Name == res.Name && n.Type == res.Type );
            if( exist != null ) {
                Program.needStop = true;
                Console.WriteLine( "存在重复文件名文件" + res.Url );
            }
            globalRes.Add( res );
        }

        //temp那种文件夹使用
        // folderPath .../assets/temp/
        // urlRoot assets/temp/
        //temp sound那种文件夹，根据文件夹下子文件夹生成组，一个文件夹一组
        public static void Folder2Groups( string folderPath, string urlRoot, List<Group> listGroup ) {
            if( Directory.Exists( folderPath ) ) {
                var allFolders = Directory.GetDirectories( folderPath, "*.*", SearchOption.TopDirectoryOnly );
                for( int i = 0; i < allFolders.Length; i++ ) {
                    var path = allFolders[i];
                    Group group = FolderGenerate.Folder2Group( path, urlRoot );
                    listGroup.Add( group );
                }
            }
        }

        //singles那种文件夹使用
        // folderPath .../assets/temp/avatar
        // urlRoot       assets/temp/(avatar)
        public static Group Folder2Group( string folderPath, string urlRoot ) {
            Group group = new Group();
            DirectoryInfo folder = new DirectoryInfo( folderPath );
            group.Name = folder.Name;//temp下子文件夹
            List<Res> list = new List<Res>();

            var newUrl = folder.Name;
            if( urlRoot != "" ) {
                newUrl = urlRoot + "/" + folder.Name;
            }
            FolderGenerate.Folder2Reses( folderPath, newUrl, list );
            group.listRes = list;
            group.Keys = group.GenerateKey();
            return group;
        }

        public static List<Res> Folder2Reses( string folderPath, string urlRoot, List<Res> list ) {
            FolderGenerate.Folder2Res( folderPath, urlRoot, list );
            var allFolders = Directory.GetDirectories( folderPath, "*.*", SearchOption.TopDirectoryOnly );
            for( int i = 0; i < allFolders.Length; i++ ) {
                var path = allFolders[i];
                DirectoryInfo folder = new DirectoryInfo( path );
                var folderName = folder.Name;
                var newUrl = folderName;
                if( urlRoot != "" ) {
                    newUrl = urlRoot + "/" + folderName;
                }
                FolderGenerate.Folder2Reses( path, newUrl, list );
            }
            return list;
        }

        //"url": "assets/singles/City1.png",
        //"type": "image",
        //"name": "City1"
        public static List<Res> Folder2Res( string folderPath, string urlRoot, List<Res> list ) {
            DirectoryInfo folder = new DirectoryInfo( folderPath );
            FileInfo[] files = folder.GetFiles();
            foreach( var item in files ) {
                var ext = item.Extension;
                var extension = ext.ToLower();
                var name = item.Name.Split( '.' );
                var fileName = name[0];

                Res res = new Res();

                res.Url = item.Name + VersionUtils.GetCRC32();
                if( urlRoot != "" ) {
                    res.Url = urlRoot + "/" + res.Url;
                }
                res.Name = fileName;

                if( extension == ".png" || extension == ".jpg" ) {
                    var existImg = list.Find( n => n.Name == fileName && n.Type == "image" );
                    if( existImg != null ) {
                        Program.needStop = true;
                        Console.WriteLine( "存在重复文件名文件" + urlRoot + "/" + item.Name );
                    }

                    var jsonImg = list.Find( n => n.Name == fileName && n.Type == "json" );
                    if( jsonImg != null ) {
                        //龙骨文件 _png _json
                        res.Name = fileName + "_png";
                        jsonImg.Name = fileName + "_json";
                    }
                    var fontBin = list.Find( n => n.Name == fileName && n.Type == "font" );
                    if( fontBin != null ) {
                        //字体 _png _fnt
                        res.Name = fileName;// fileName + "_png";singles里的字体文件BattleDMG
                        fontBin.Name = fileName + "_fnt";
                    }
                    res.Type = "image";
                }
                else if( extension == ".json" ) {
                    var dragonImg = list.Find( n => n.Name == fileName && n.Type == "image" );
                    if( dragonImg != null ) {
                        //龙骨文件 _png _json
                        res.Name = fileName + "_json";
                        dragonImg.Name = fileName + "_png";
                    }
                    res.Type = "json";
                }
                else if( extension == ".txt" ) {

                    res.Type = "text";
                }
                else if( extension == ".ttf" || extension == ".font" || extension == ".fnt" ) {
                    string tempName = "";
                    for( int i = 0; i < name.Length - 1; i++ ) {
                        tempName += name[i];
                        if( i < name.Length - 2 ) {
                            tempName += "_";
                        }
                    }
                    res.Name = tempName;

                    var fontImg = list.Find( n => n.Name == fileName && n.Type == "image" );
                    if( fontImg != null ) {
                        //字体 _png _fnt
                        res.Name = fileName + "_fnt";
                        fontImg.Name = fileName;// fileName + "_png";singles里的字体文件BattleDMG
                    }

                    res.Type = "font";
                }
                else if( extension == ".mp3" || extension == ".wav" || extension == ".ogg" ) {

                    res.Type = "sound";
                }
                else if( extension == ".spb" || extension == ".sproto" ) {
                    res.Name = fileName + "_" + extension.Replace( ".", "" );

                    res.Type = "bin";
                }
                else {
                    //其他识别为二进制
                    //还有一个sheet

                    res.Type = "bin";
                }
                list.Add( res );

                //检测重名
                FolderGenerate.CheckRepeat( res );
            }
            return list;
        }

    }
}
