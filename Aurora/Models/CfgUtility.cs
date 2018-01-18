using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aurora.Models
{
    public class CfgUtility
    {
        public static Dictionary<string, string> GetSysConfig(Controller ctrl)
        {
            var lines = System.IO.File.ReadAllLines(ctrl.Server.MapPath("~/Scripts/AuroraCfg.txt"));
            var ret = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                if (line.Contains("##"))
                {
                    continue;
                }

                if (line.Contains(":::"))
                {
                    var kvpair = line.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
                    if (!ret.ContainsKey(kvpair[0].Trim()))
                    {
                        ret.Add(kvpair[0].Trim(), kvpair[1].Trim());
                    }
                }//end if
            }//end foreach
            return ret;
        }

        public static List<string> GetEmployeeList(Controller ctrl)
        {
            var lines = System.IO.File.ReadAllLines(ctrl.Server.MapPath("~/Scripts/AuroraCfg.txt"));
            var ret = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                if (line.Contains("##"))
                {
                    continue;
                }

                if (line.Contains(":::"))
                {
                    var kvpair = line.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
                    if (!ret.ContainsKey(kvpair[1].Trim())
                        && (kvpair[0].Trim().ToUpper().Contains("WUX-L")
                        || kvpair[0].Trim().ToUpper().Contains("WUX-D")
                        || kvpair[0].Trim().ToUpper().Contains("SHG-L")
                        || kvpair[0].Trim().ToUpper().Contains("SHG-D"))
                        ){
                        ret.Add(kvpair[1].Trim(), kvpair[0].Trim());
                    }
                }//end if
            }//end foreach

            var retlist = new List<string>(ret.Keys);
            retlist.Sort();
            return retlist;
        }
    }
}