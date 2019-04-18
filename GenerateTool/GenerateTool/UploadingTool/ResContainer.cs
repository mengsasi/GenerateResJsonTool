using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateTool.UploadingTool {

    class ResContainer {

        public static List<Res> ListResLocalAll = new List<Res>();

        public static void AddReses( List<Group> list ) {
            var listRes = new List<Res>();
            for( int i = 0; i < list.Count; i++ ) {
                var l = list[i].listRes;
                for( int j = 0; j < l.Count; j++ ) {
                    var res = l[j];
                    res.group = list[i];
                }
                listRes.AddRange( l );
            }
            //记录所有本地文件Res
            ResContainer.ListResLocalAll = new List<Res>();
            ResContainer.ListResLocalAll.AddRange( listRes );
        }

    }
}
