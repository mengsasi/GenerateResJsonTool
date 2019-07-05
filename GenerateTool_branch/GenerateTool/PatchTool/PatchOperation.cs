using Force.Crc32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace GenerateTool.PatchTool {

    class PatchOperation {

        public static void MainPatch(JToken configJToken) {

            var initPath = configJToken["initPath"].Value<string>();
            var initVersionPath = configJToken["initVersionPath"].Value<string>();
            var patchPath = configJToken["patchPath"].Value<string>();
            var patchVersionPath = configJToken["patchVersionPath"].Value<string>();

            var rootPath = Program.ProjectRootPath;


            #region 生成初始版本

            var initDirPath = rootPath + "\\" + initPath;//原版本resource文件夹路径
            var initVersionFilePath = rootPath + "\\" + initVersionPath;//生成的 init.txt 路径

            using(TextWriter writer = new StreamWriter(initVersionFilePath)) {
                var roots = Directory.GetDirectories(initDirPath);
                foreach(var root in roots) {
                    var files = Directory.GetFiles(root, "*.*", SearchOption.AllDirectories);
                    foreach(var file in files) {
                        FileStream stream = new FileStream(file, FileMode.Open);
                        byte[] buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, (int)stream.Length);
                        stream.Close();
                        uint crc32 = Crc32Algorithm.Compute(buffer);
                        Console.WriteLine("处理文件：{0}-{1}", Path.GetFileName(file), crc32);

                        var fileRelatePath = file.Substring(initDirPath.Length + 1);
                        fileRelatePath = fileRelatePath.Replace('\\', '/');
                        writer.WriteLine("{0}:{1}", fileRelatePath, crc32);
                    }
                }
            }

            #endregion


            #region 生成patch文件

            //删除之前创建的
            string gangsterRes000 = "gangsterRes";// Program.VersionFolder;
            var lastResDir = rootPath + "\\" + gangsterRes000;
            if(Directory.Exists(lastResDir)) {
                Directory.Delete(lastResDir, true);
            }

            var patchDirPath = rootPath + "\\" + patchPath;//新版本resource文件夹路径
            var patchVersionFilePath = rootPath + "\\" + patchVersionPath;//生成的 1.2.0.txt 路径

            Dictionary<string, uint> initFileCrc = new Dictionary<string, uint>();
            using(TextReader reader = new StreamReader(initVersionFilePath)) {
                while(true) {
                    var line = reader.ReadLine();
                    if(string.IsNullOrEmpty(line)) {
                        break;
                    }

                    var parts = line.Split(':');
                    var fileRelatePath = parts[0];
                    uint crc32 = uint.Parse(parts[1]);

                    initFileCrc.Add(fileRelatePath, crc32);
                }
            }

            using(TextWriter patchWriter = new StreamWriter(patchVersionFilePath)) {
                var roots = Directory.GetDirectories(patchDirPath);
                foreach(var root in roots) {
                    var files = Directory.GetFiles(root, "*.*", SearchOption.AllDirectories);
                    foreach(var file in files) {
                        FileStream stream = new FileStream(file, FileMode.Open);
                        byte[] buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, (int)stream.Length);
                        stream.Close();

                        uint crc32 = Crc32Algorithm.Compute(buffer);
                        var fileRelatePath = file.Substring(patchDirPath.Length + 1);
                        fileRelatePath = fileRelatePath.Replace('\\', '/');
                        if(initFileCrc.ContainsKey(fileRelatePath)) {
                            if(initFileCrc[fileRelatePath] == crc32) {
                                continue;
                            }
                        }
                        Console.WriteLine("新增Patch文件：{0}-{1}", Path.GetFileName(file), crc32);
                        patchWriter.WriteLine("{0}:{1}", fileRelatePath, crc32);

                        if(file.Contains("exml") || file.Contains("Web") || file.Contains("web")) {
                            continue;
                        }

                        //复制修改的文件到指定文件夹
                        PatchOperation.CopyToNewPath(file, fileRelatePath);

                    }
                }
            }
            Console.WriteLine("生成Patch文件成功");

            #endregion

        }

        private static void CopyToNewPath(string sourcePath, string targetRelate) {
            string root = Program.ProjectRootPath;
            string gangsterRes000 = "gangsterRes";// Program.VersionFolder;
            var targetPath = root + "\\" + gangsterRes000 + "\\" + targetRelate;
            PatchOperation.MakeDir(targetPath);
            File.Copy(sourcePath, targetPath, true);
        }

        private static void MakeDir(string filePath) {
            var root = Program.ProjectRootPath.Replace('\\', '/');
            string path = filePath.Replace('\\', '/');
            var fileRelatePath = PatchOperation.GetRelatePath(filePath, root);

            var dirs = fileRelatePath.Split('/');
            var dirPath = root;
            for(int i = 0; i < dirs.Length - 1; i++) {
                string dir = dirs[i];
                if(dir == "") {
                    if(!Directory.Exists(dirPath)) {
                        Directory.CreateDirectory(dirPath);
                    }
                }
                else {
                    dirPath = dirPath + "/" + dir;
                    if(!Directory.Exists(dirPath)) {
                        Directory.CreateDirectory(dirPath);
                    }
                }
            }
        }

        public static string GetRelatePath(string filePath, string rootPath) {
            var fileRelatePath = filePath.Substring(rootPath.Length);
            fileRelatePath = fileRelatePath.Replace('\\', '/');
            return fileRelatePath;
        }


    }
}
