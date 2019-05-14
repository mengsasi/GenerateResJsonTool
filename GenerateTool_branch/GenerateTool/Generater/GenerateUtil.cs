using GenerateTool.UploadingTool;
using Newtonsoft.Json.Linq;
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
        }

        public static void GenerateWeb( JToken configJToken, string resourcePath, string filePath ) {
            List<Group> listGroup = new List<Group>();
            GenerateUtil.GenerateConfig( listGroup, configJToken["gangsterGroups"], resourcePath );

            if( AssetOperation.isGenerate && AssetOperation.ListChangeGroup.Count > 0 ) {
                listGroup.AddRange( AssetOperation.ListChangeGroup );
            }
            //生成文件
            GenerateUtil.GenerateFile( filePath, listGroup );
        }

        public static void GenerateConfig( List<Group> listGroup, JToken groupConfigs, string resourcePath ) {
            if( groupConfigs == null ) {
                Program.ConsoleLog( "缺少配置" );
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
                        if( singleGroup != null ) {
                            SpecialFolderGenerate.CheckDelGroup( singleGroup, listGroup );
                            listGroup.Add( singleGroup );
                        }
                    }
                    else {
                        SpecialFolderGenerate.SheetFolder2Groups( resourcePath + confFolderPath, confUrlRoot, listGroup );
                    }
                }
                else {
                    if( isSingle ) {
                        var singleGroup = FolderGenerate.Folder2Group( resourcePath + confFolderPath, confUrlRoot );
                        if( singleGroup != null ) {
                            listGroup.Add( singleGroup );
                        }
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
            json += "{";
            json += Group.Generate( listGroup );
            json += Res.Generate( listRes );
            json += "}";
            FileUtil.Save( filePath, json );
        }

    }


    #region 生成Group

    public class Group {
        public string Keys;
        public string Name;
        public List<Res> listRes = new List<Res>();

        public string Group2Json() {
            string json = "";
            json += "{\"keys\":\"";
            json += Keys;
            json += "\",\"name\":\"";
            json += Name;
            json += "\"}";
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

        public static string Generate( List<Group> listGroup ) {
            string json = "";
            json += "\"groups\":[";
            string group = "";
            for( int i = 0; i < listGroup.Count; i++ ) {
                group += listGroup[i].Group2Json();
                if( i < listGroup.Count - 1 ) {
                    group += ",";
                }
            }
            json += group;
            json += "],";
            return json;
        }
    }

    #endregion


    #region 生成resources

    public class Res {
        public string Url;
        public string Type;
        public string Name;
        public string SubKeys = "";

        public Group group;

        public string Res2Json() {
            string json = "";
            json += "{";
            json += "\"url\": \"" + Replace( Url ) + "\"";
            json += ",";
            json += "\"type\": \"" + Type + "\"";
            json += ",";
            json += "\"name\": \"" + Name + "\"";
            if( SubKeys != "" ) {
                json += ",";
                json += "\"subkeys\": \"" + SubKeys + "\"";
            }
            json += "}";
            return json;
        }

        static string Replace( string text ) {
            return text.Replace( '\\', '/' );
        }

        public static string Generate( List<Res> list ) {
            string json = "";
            json += "\"resources\":[";
            for( int i = 0; i < list.Count; i++ ) {
                var res = list[i];
                json += res.Res2Json();
                if( i < list.Count - 1 ) {
                    json += ",";
                }
            }
            json += "]";
            return json;
        }
    }

    #endregion


}
