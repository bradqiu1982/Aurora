using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Aurora.Models;

namespace Aurora.Controllers
{
    public class CoWorkController : Controller
    {
        public static string DetermineCompName(string IP)
        {
            try
            {
                IPAddress myIP = IPAddress.Parse(IP);
                IPHostEntry GetIPHost = Dns.GetHostEntry(myIP);
                List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
                return compName.First();
            }
            catch (Exception ex)
            { return string.Empty; }
        }

        private void UserAuth()
        {
            string IP = Request.UserHostName;
            var compName = DetermineCompName(IP);
            ViewBag.compName = compName;
            var glbcfg = CfgUtility.GetSysConfig(this);
            if (glbcfg.ContainsKey(ViewBag.compName))
            {
                ViewBag.compName = glbcfg[ViewBag.compName].Trim().ToUpper();
            }
        }
        // GET: CoWork
        public ActionResult Home()
        {
            UserAuth();
            var employlist = CfgUtility.GetEmployeeList(this);
            ViewBag.EmployeeList = Newtonsoft.Json.JsonConvert.SerializeObject(employlist.ToArray());
            var pjlist = CfgUtility.GetPJList(this);
            ViewBag.PJList = Newtonsoft.Json.JsonConvert.SerializeObject(pjlist.ToArray());
            return View();
        }
    }
}