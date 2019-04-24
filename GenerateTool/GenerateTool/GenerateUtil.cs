using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace GenerateTool {

    class GenerateUtil {

        public static void GenerateLocal( JToken configJToken, string resourcePath, string filePath ) {

            List<Group> listGroup = new List<Group>();

            GenerateUtil.GenerateConfig( listGroup, configJToken["assetsGroups"], resourcePath );

            //loading组
            var loadingGroup = listGroup.Find( item => item.Name == "loading" );
            if( loadingGroup != null ) {
                List<Res> listPreload = new List<Res>();

                var listConfig = SpecialFolderGenerate.GenerateConfig( resourcePath );
                var listFont = SpecialFolderGenerate.GenerateFont( resourcePath );

                listPreload.AddRange( listConfig );
                listPreload.AddRange( listFont );

                loadingGroup.listRes.AddRange( listPreload );
                loadingGroup.Keys = loadingGroup.GenerateKey();
            }

            //生成文件
            GenerateUtil.GenerateFile( filePath, listGroup );

            //记录所有本地文件Res
            UploadingTool.ResContainer.AddReses( listGroup );
        }

        public static void GenerateWeb( JToken configJToken, string resourcePath, string filePath ) {

            List<Group> listGroup = new List<Group>();

            GenerateUtil.GenerateConfig( listGroup, configJToken["gangsterGroups"], resourcePath );

            if( UploadingTool.MainUploading.isUploading ) {
                //添加修改的本地组
                listGroup.AddRange( UploadingTool.MainUploading.ListChangeGroup );
            }

            //生成文件
            GenerateUtil.GenerateFile( filePath, listGroup );
        }

        public static void GenerateConfig( List<Group> listGroup, JToken groupConfigs, string resourcePath ) {
            if( groupConfigs == null ) {
                Console.WriteLine( "缺少配置" );
                Program.needStop = true;
                return;
            }
            foreach( var child in groupConfigs ) {
                var confFolderPath = child["folderPath"].Value<string>();
                var confUrlRoot = child["urlRoot"].Value<string>();
                var isSingle = child["single"].Value<bool>();
                var isSheet = false;
                if( child["sheet"] != null ) {
                    isSheet = child["sheet"].Value<bool>();
                }
                if( isSheet ) {
                    if( isSingle ) {
                        var singleGroup = SpecialFolderGenerate.SheetFolder2Group( resourcePath + confFolderPath, confUrlRoot );
                        SpecialFolderGenerate.CheckDelGroup( singleGroup, listGroup );
                        listGroup.Add( singleGroup );
                    }
                    else {
                        SpecialFolderGenerate.SheetFolder2Groups( resourcePath + confFolderPath, confUrlRoot, listGroup );
                    }
                }
                else {
                    if( isSingle ) {
                        var singleGroup = FolderGenerate.Folder2Group( resourcePath + confFolderPath, confUrlRoot );
                        listGroup.Add( singleGroup );
                    }
                    else {
                        FolderGenerate.Folder2Groups( resourcePath + confFolderPath, confUrlRoot, listGroup );
                    }
                }
            }
        }

        public static void GenerateFile( string filePath, List<Group> listGroup ) {
            var listRes = new List<Res>();
            for( int i = 0; i < listGroup.Count; i++ ) {
                var l = listGroup[i].listRes;
                listRes.AddRange( l );
            }
            string json = "";
            json = json.AppendLeftBrace().AppendBr();
            json += Groups.Generate( listGroup );
            json += Resources.Generate( listRes );
            json = json.AppendRightBrace();

            FileUtil.Save( filePath, json );
        }

    }


    #region 生成Group

    class Groups {

        public static string Generate( List<Group> listGroup ) {
            string json = "";
            json += "\t\"groups\": [";
            json += "\n";

            string group = "";
            for( int i = 0; i < listGroup.Count; i++ ) {
                group += listGroup[i].Group2Json();
                if( i < listGroup.Count - 1 ) {
                    group = group.AppendComma();
                }
                group = group.AppendBr();
            }

            json += group;

            json += "\t],";
            json += "\n";
            return json;
        }
    }

    class Group {
        public string Keys;
        public string Name;
        public List<Res> listRes = new List<Res>();

        public string Group2Json() {
            string json = "";
            json = json.AppendTap( 2 ).AppendLeftBrace().AppendBr();
            json = json.AppendTap( 3 ).AppendMark();
            json = json += "keys";
            json = json.AppendMark().AppendColon().AppendSpace().AppendMark();
            json += Keys;
            json = json.AppendMark().AppendComma().AppendBr();
            json = json.AppendTap( 3 ).AppendMark();
            json = json += "name";
            json = json.AppendMark().AppendColon().AppendSpace().AppendMark();
            json += Name;
            json = json.AppendMark().AppendBr();
            json = json.AppendTap( 2 ).AppendRightBrace();
            return json;
        }

        public string GenerateKey() {
            string keys = "";
            for( int i = 0; i < listRes.Count; i++ ) {
                keys += listRes[i].Name;
                if( i < listRes.Count - 1 ) {
                    keys += ",";
                }
            }
            return keys;
        }
    }

    #endregion 


    #region 生成resources

    class Resources {
        public static string Generate( List<Res> list ) {
            string json = "";
            json += "\t\"resources\": [";
            json += "\n";
            for( int i = 0; i < list.Count; i++ ) {
                var res = list[i];
                json += res.Res2Json();
                if( i < list.Count - 1 ) {
                    json += ",";
                }
                json += "\n";
            }
            json += "\t]";
            json += "\n";
            return json;
        }
    }

    class Res {
        public string Url;
        public string Type;
        public string Name;
        public string SubKeys = "";
        //public string SoundType = "";

        public Group group;

        public string Res2Json() {
            string json = "";
            json += "\t\t{";
            json += "\n";
            json += "\t\t\t";
            json += "\"url\": \"" + Replace( Url ) + "\"";
            json += ",";
            json += "\n";
            json += "\t\t\t";
            json += "\"type\": \"" + Type + "\"";
            json += ",";
            json += "\n";
            json += "\t\t\t";
            json += "\"name\": \"" + Name + "\"";
            //if( SoundType != "" ) {
            //    json += ",";
            //    json += "\n";
            //    json += "\t\t\t";
            //    json += "\"soundType\": \"" + SoundType + "\"";
            //}
            if( SubKeys != "" ) {
                json += ",";
                json += "\n";
                json += "\t\t\t";
                json += "\"subkeys\": \"" + SubKeys + "\"";
            }
            json += "\n";
            json += "\t\t}";
            return json;
        }

        static string Replace( string text ) {
            return text.Replace( '\\', '/' );
        }
    }

    #endregion


}
