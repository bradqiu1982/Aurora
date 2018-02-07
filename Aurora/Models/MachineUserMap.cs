using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aurora.Models
{
    public class MachineUserMap
    {
        public MachineUserMap()
        {
            machine = "";
            username = "";
        }

        public static void UpdateMachineUserMap(string machine, string username)
        {
            var sql = "delete from aurorausermap where machine = '<machine>'";
            sql = sql.Replace("<machine>", machine);
            DBUtility.ExeLocalSqlNoRes(sql);
            sql = "insert into aurorausermap(machine,username) values(@machine,@username)";
            var param = new Dictionary<string, string>();
            param.Add("@machine", machine);
            param.Add("@username", username);
            DBUtility.ExeLocalSqlNoRes(sql, param);
        }

        public static List<MachineUserMap> RetrieveUserMap()
        {
            var ret = new List<MachineUserMap>();

            var sql = "select machine,username from aurorausermap";
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var tempvm = new MachineUserMap();
                tempvm.machine = Convert.ToString(line[0]);
                tempvm.username = Convert.ToString(line[1]);
                ret.Add(tempvm);
            }
            return ret;
        }

        public string machine { set; get; }
        public string username { set; get; }
    }
}