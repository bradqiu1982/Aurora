﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Aurora.Models;
using System.Web.Routing;

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
                ViewBag.username = glbcfg[ViewBag.compName].Trim().ToUpper();
            }
            else
            {
                ViewBag.username = ViewBag.compName;
            }
        }

        // GET: CoWork
        public ActionResult Home(string activenavitem,string topicid)
        {
            UserAuth();
            var employlist = new List<string>();
            employlist.AddRange(CoTopicVM.GetNewPeopleList());
            employlist.AddRange(CfgUtility.GetEmployeeList(this));
            ViewBag.EmployeeList = Newtonsoft.Json.JsonConvert.SerializeObject(employlist.ToArray());

            var pjlist = new List<string>();
            pjlist.AddRange(CoTopicVM.GetNewPJList());
            pjlist.AddRange(CfgUtility.GetPJList(this));
            ViewBag.PJList = Newtonsoft.Json.JsonConvert.SerializeObject(pjlist.ToArray());
            ViewBag.TopicId = CoTopicVM.GetUniqKey();

            var tempnavlist = new List<string>();
            tempnavlist.AddRange(new string[] { TopicBelongType.IAssign, TopicBelongType.IRelated, TopicBelongType.Completed });
            ViewBag.NavList = tempnavlist;
            if (string.IsNullOrEmpty(activenavitem)) 
            {
                ViewBag.ActiveNav = tempnavlist[0];
            }
            else {
                ViewBag.ActiveNav = activenavitem;
            }

            if (string.Compare(ViewBag.ActiveNav, TopicBelongType.Completed) == 0)
            {
                ViewBag.topiclist = CoTopicVM.RetrieveCompleteTopic4List(ViewBag.username);
            }
            else
            {
                ViewBag.topiclist = CoTopicVM.RetrieveTopic4List(ViewBag.username, ViewBag.ActiveNav);
            }

            
            if (!string.IsNullOrEmpty(topicid))
            {
                ViewBag.ActiveTopicid = topicid;
                ViewBag.CurrentTopic = CoTopicVM.RetrieveTopic(ViewBag.ActiveTopicid);
            }
            else
            {
                if (ViewBag.topiclist.Count > 0)
                {
                    ViewBag.ActiveTopicid = ViewBag.topiclist[0].topicid;
                    ViewBag.CurrentTopic = CoTopicVM.RetrieveTopic(ViewBag.ActiveTopicid);
                }
            }

            return View();
        }

        public JsonResult TopicByID()
        {
            var topicid = Request.Form["topicid"];
            var tempvm = CoTopicVM.RetrieveTopic(topicid);
            var ret = new JsonResult();
            if (tempvm.Count > 0)
            {
                ret.Data = new { sucess = true
                                ,data = tempvm[0]};
            }
            else
            {
                ret.Data = new { sucess = false };
            }

            return ret;
        }

        public ActionResult CreateNewTopic()
        {
            UserAuth();
            var employlist = new List<string>();
            employlist.AddRange(CoTopicVM.GetNewPeopleList());
            employlist.AddRange(CfgUtility.GetEmployeeList(this));
            ViewBag.EmployeeList = Newtonsoft.Json.JsonConvert.SerializeObject(employlist.ToArray());

            var pjlist = new List<string>();
            pjlist.AddRange(CoTopicVM.GetNewPJList());
            pjlist.AddRange(CfgUtility.GetPJList(this));
            ViewBag.PJList = Newtonsoft.Json.JsonConvert.SerializeObject(pjlist.ToArray());
            ViewBag.TopicId = CoTopicVM.GetUniqKey();
            return View();
        }


        [HttpPost]
        public ActionResult NewTopic()
        {
            UserAuth();

            var topicid = Request.Form["topicid"];
            if (!string.IsNullOrEmpty(Request.Form["JobTopicEditor"]))
            {
                var topiccontent = SeverHtmlDecode.Decode(this, Request.Form["JobTopicEditor"]);
                var subject = Request.Form["subject"];
                CoTopicVM.AddNewTopic(topicid,subject, topiccontent, ViewBag.username, ViewBag.compName);
            }

            return RedirectToAction("Home", "CoWork");
        }

        public JsonResult NewTopicDueDate()
        {
            var topicid = Request.Form["topicid"];
            var duedate = Request.Form["duedate"];
            CoTopicVM.UpdateTopicDueDate(topicid, duedate);

            var ret = new JsonResult();
            ret.Data = new { sucess = true };
            return ret;
        }

        public JsonResult NewTopicPJ()
        {
            var topicid = Request.Form["topicid"];
            var pjs = Request.Form["pjs"];
            var splitstrs = pjs.Split(new string[] { " #" }, StringSplitOptions.RemoveEmptyEntries);
            var pjlist = new List<string>();
            foreach (var pj in splitstrs)
            {
                pjlist.Add(pj.Replace("#", "").Trim());
            }
            TopicProject.UpdateTopicPJ(topicid, pjlist, this);

            var ret = new JsonResult();
            ret.Data = new { sucess = true };
            return ret;
        }

        public JsonResult NewTopicPeople()
        {
            var topicid = Request.Form["topicid"];
            var pps = Request.Form["pps"];
            var splitstrs = pps.Split(new string[] { " @" }, StringSplitOptions.RemoveEmptyEntries);
            var pplist = new List<string>();
            foreach (var pj in splitstrs)
            {
                pplist.Add(pj.Replace("@", "").Trim());
            }
            CoTopicVM.UpdateTopicPeople(topicid, pplist, this);

            var ret = new JsonResult();
            ret.Data = new { sucess = true };
            return ret;
        }

        [HttpPost]
        public ActionResult NewTopicComment()
        {
            UserAuth();
            var activetopicid = Request.Form["activetopicid"];
            var activenavitem = Request.Form["activenav"];
            var commentcontent = SeverHtmlDecode.Decode(this, Request.Form["CommentEditor"]);
            if (!string.IsNullOrEmpty(commentcontent))
            {
                var commenttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var commentid = CoTopicVM.GetUniqKey();

                TopicCommentVM.AddComment(activetopicid, commentid, commentcontent, ViewBag.username, commenttime);
            }

            var routedict = new RouteValueDictionary();
            routedict.Add("activenavitem", activenavitem);
            routedict.Add("topicid", activetopicid);
            return RedirectToAction("Home", "CoWork", routedict);
        }

    }
}