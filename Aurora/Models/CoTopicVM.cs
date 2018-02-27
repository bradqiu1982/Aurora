using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aurora.Models
{
    public class TopicPJStatus
    {
        public static string Working = "WORKING";
        public static string Done = "DONE";
    }

    public class TopicStatus
    {
        public static string Working = "WORKING";
        public static string Done = "DONE";
    }

    public class TopicBelongType
    {
        public static string IAssign = "I-Assign";
        public static string IRelated = "I-Related";
        public static string Completed = "Completed";
        public static string ICreate = "I-Create";
        public static string IModify = "I-Modify";
    }

    public class EVENTWARNINGCLOCK {
        public static string EVERYMONTH = "WARNING EVERY MONTH";
        public static string EVERYDAY = "WARNING EVERY DAY AFTER DUE DATE";
        public static string EVERYWEEK = "WARNING EVERY WEEK AFTER DUE DATE";
    }

    public class EventTimer {
        public EventTimer()
        {
            duedate = "";
            warningclock = "";
        }

        public EventTimer(string dt, string wl)
        {
            duedate = dt;
            warningclock = wl;
        }

        public string duedate { set; get; }
        public string warningclock { set; get; }
    }

    public class SeverHtmlDecode
    {
        private static string WriteBase64ImgFile(string commentcontent, Controller ctrl)
        {
            try
            {
                var idx = commentcontent.IndexOf("<img alt=\"\" src=\"data:image/png;base64");
                var base64idx = commentcontent.IndexOf("data:image/png;base64,", idx) + 22;
                var base64end = commentcontent.IndexOf("\"", base64idx);
                var imgstrend = commentcontent.IndexOf("/>", base64end) + 2;
                var base64img = commentcontent.Substring(base64idx, base64end - base64idx);
                var imgbytes = Convert.FromBase64String(base64img);

                var imgkey = Guid.NewGuid().ToString("N");
                string datestring = DateTime.Now.ToString("yyyyMMdd");
                string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\images\\" + datestring + "\\";

                if (!Directory.Exists(imgdir))
                {
                    Directory.CreateDirectory(imgdir);
                }
                var realpath = imgdir + imgkey + ".jpg";

                var fs = File.Create(realpath);
                fs.Write(imgbytes, 0, imgbytes.Length);
                fs.Close();


                var url = "/userfiles/images/" + datestring + "/" + imgkey + ".jpg";
                var ret = commentcontent;
                ret = ret.Remove(idx, imgstrend - idx);
                ret = ret.Insert(idx, "<img src='" + url + "'/>");

                return ret;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }

        private static string ReplaceBase64data2File(string commentcontent, Controller ctrl)
        {
            var ret = commentcontent;
            if (commentcontent.Contains("<img alt=\"\" src=\"data:image/png;base64"))
            {
                while (ret.Contains("<img alt=\"\" src=\"data:image/png;base64"))
                {
                    ret = WriteBase64ImgFile(ret, ctrl);
                    if (string.IsNullOrEmpty(ret))
                    {
                        ret = commentcontent;
                        break;
                    }
                }
            }

            return ret;
        }

        public static string Decode(Controller ctrl, string src)
        {
            var ret = ctrl.Server.HtmlDecode(src).Replace("border=\"0\"", "border=\"2\"");
            ret = ReplaceBase64data2File(ret, ctrl);
            return ret;
        }

        private static string removeimgstr(string src)
        {
            var tempstr = src;
            var imgidx = tempstr.IndexOf("<img src=");
            var imgendidx = tempstr.IndexOf("/>",imgidx + 1)+2;
            if (imgidx != -1 && imgendidx != -1)
            {
                return tempstr.Remove(imgidx, (imgendidx - imgidx));
            }
            else
            {
                return tempstr.Remove(imgidx+1,3);
            }
        }

        public static string RemoveImageFromHtml(string src)
        {
            var tempstr = src;
            if (tempstr.Contains("<img src="))
            {
                while (tempstr.Contains("<img src="))
                {
                    tempstr = removeimgstr(tempstr);
                }
            }
            return tempstr;
        }

        public static string NoTagContent(string src)
        {
            var des = System.Text.RegularExpressions.Regex.Replace(src.Replace("\"", "").Replace("&nbsp;", ""), "<.*?>", string.Empty).Trim();
            return (des.Length > 180) ? des.Substring(0, 180) : des;
        }


        //private static string resizeimg(string imgstr)
        //{
        //    var splitstr = imgstr.Split(new string[] { "width=\""}, StringSplitOptions.RemoveEmptyEntries);
        //    var width = splitstr[1].Split(new string[] { "\""},StringSplitOptions.RemoveEmptyEntries)[0];
        //    splitstr = imgstr.Split(new string[] { "height=\""}, StringSplitOptions.RemoveEmptyEntries);
        //    var height = splitstr[1].Split(new string[] { "\"" }, StringSplitOptions.RemoveEmptyEntries)[0];

        //    var iw = Convert.ToDouble(width);
        //    var ih = Convert.ToDouble(height);
        //    if (iw > 700)
        //    {
        //        var srcidx = imgstr.IndexOf("src=\"");
        //        var srceidx = imgstr.IndexOf("\"", srcidx + 6);
        //        var srcstr = imgstr.Substring(srcidx, (srceidx + 1 - srcidx));
        //        var newh = Convert.ToInt32( 700.0 / iw * ih);
        //        return "<img width=\"700\" height=\"" + newh.ToString() + "\" " + srcstr + ">";
        //    }
        //    else
        //    {
        //        return imgstr;
        //    }
        //}

        //public static string ResizeImageFromHtml(string src)
        //{
        //    var startidx = 0;
        //    while (src.IndexOf("<img", startidx) != -1)
        //    {
        //        var imgsidx = src.IndexOf("<img",startidx);
        //        var imgeidx = src.IndexOf(">", imgsidx);
        //        if (imgeidx != -1)
        //        {
        //            startidx = imgeidx;
        //            imgeidx = imgeidx + 1;
        //            var imgstr = src.Substring(imgsidx, (imgeidx - imgsidx));
        //            if (imgstr.Contains("width=\"") && imgstr.Contains("height=\""))
        //            {
        //                var nimgstr = resizeimg(imgstr);
        //                src = src.Remove(imgsidx, imgeidx - imgsidx).Insert(imgsidx, nimgstr);
        //            }
        //        }
        //        else
        //        {
        //            startidx = imgsidx+3;
        //        }
        //    }
        //    return src;
        //}

        private static string resizeimg(string imgstr)
        {
            var srcidx = imgstr.IndexOf("src=\"");
            var srceidx = imgstr.IndexOf("\"", srcidx + 6);
            var srcstr = imgstr.Substring(srcidx, (srceidx + 1 - srcidx));
            return "<div style=\"text-align: center;\">" + "<img " + srcstr + " style=\"max-width: 90%; height: auto;\" /></div>";
        }
        public static string ResizeImageFromHtml(string src)
        {
            var startidx = 0;
            while (src.IndexOf("<img", startidx) != -1)
            {
                var imgsidx = src.IndexOf("<img", startidx);
                var imgeidx = src.IndexOf(">", imgsidx);
                if (imgeidx != -1)
                {
                    startidx = imgeidx;
                    imgeidx = imgeidx + 1;
                    var imgstr = src.Substring(imgsidx, (imgeidx - imgsidx));
                    var nimgstr = resizeimg(imgstr);
                    src = src.Remove(imgsidx, imgeidx - imgsidx).Insert(imgsidx, nimgstr);
                }
                else
                {
                    startidx = imgsidx + 3;
                }
            }
            return src.Replace("</img>","");
        }

    }


    public class CoTopicVM
    {
        public static string GetUniqKey()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static void AddNewTopic(string topicid,string subject, string topiccontent, string creator, string createmachine)
        {
            var sql = "insert into CoTopicVM(topicid,subject,topiccontent,creator,createmachine,status,createdate,Removed) values(@topicid,@subject,@topiccontent,@creator,@createmachine,@status,@createdate,'FALSE')";
            var param = new Dictionary<string, string>();
            param.Add("@topicid", topicid);
            param.Add("@subject", subject);
            param.Add("@topiccontent", topiccontent);
            param.Add("@creator", creator);
            param.Add("@createmachine", createmachine);
            param.Add("@status", TopicStatus.Working);
            param.Add("@createdate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            DBUtility.ExeLocalSqlNoRes(sql, param);

            sql = "insert into auroratopicpeople(topicid,people,isowner) values('<topicid>','<people>','TRUE')";
            sql = sql.Replace("<topicid>", topicid).Replace("<people>", creator);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static void UpdateTopic(string tid, string cc)
        {
            var sql = "update CoTopicVM set topiccontent = @topiccontent where topicid = @topicid";
            var param = new Dictionary<string, string>();
            param.Add("@topicid", tid);
            param.Add("@topiccontent", cc);
            DBUtility.ExeLocalSqlNoRes(sql, param);
        }

        public static void UpdateTopicDueDate(string topicid,string duedate,string warningclock)
        {
            var sql = "delete from auroratopicduedate where topicid = '<topicid>'";
            sql = sql.Replace("<topicid>", topicid);
            DBUtility.ExeLocalSqlNoRes(sql);

            sql = "insert into auroratopicduedate(topicid,duedate,warningclock) values(@topicid,@duedate,@warningclock)";
            var param = new Dictionary<string, string>();
            param.Add("@topicid", topicid);
            param.Add("@duedate", duedate);
            param.Add("@warningclock", warningclock);
            DBUtility.ExeLocalSqlNoRes(sql,param);

        }

        public static List<EventTimer> RetrieveTopicDueDate(string topicid)
        {
            var ret = new List<EventTimer>();

            var sql = "select duedate,warningclock from auroratopicduedate where topicid = '<topicid>'";
            sql = sql.Replace("<topicid>", topicid);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                ret.Add(new EventTimer(Convert.ToString(line[0]),Convert.ToString(line[1])));
            }
            return ret;
        }

        public static List<string> GetNewPJList()
        {
            var ret = new List<string>();
            var sql = "select distinct project from auroranewpj";
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                ret.Add(Convert.ToString(line[0]));
            }
            return ret;
        }

        public static void UpdateTopicPeople(string topicid, List<string> relatedPeople , Controller ctrl)
        {
            var sql = "delete from auroratopicpeople where topicid = '<topicid>'";
            sql = sql.Replace("<topicid>", topicid);
            DBUtility.ExeLocalSqlNoRes(sql);

            foreach (var p in relatedPeople)
            {
                sql = "insert into auroratopicpeople(topicid,people) values(@topicid,@people)";
                var param = new Dictionary<string, string>();
                param.Add("@topicid", topicid);
                param.Add("@people", p);
                DBUtility.ExeLocalSqlNoRes(sql, param);
            }

            var pdict = CfgUtility.GetEmployeeDict(ctrl);
            foreach (var p in relatedPeople)
            {
                if (!pdict.ContainsKey(p))
                {
                    sql = "delete from auroranewpeople where people = '<people>'";
                    sql = sql.Replace("<people>", p);
                    DBUtility.ExeLocalSqlNoRes(sql);

                    sql = "insert into auroranewpeople(people) values('<people>')";
                    sql = sql.Replace("<people>", p);
                    DBUtility.ExeLocalSqlNoRes(sql);
                }
            }
       }

        public static void UpdateTopicIsRead(string topicid, string people, bool isread)
        {
            var IsRead = isread ? "TRUE" : "FALSE";
            var sql = "update auroratopicpeople set IsRead = '<IsRead>' where topicid= '<topicid>' and people = '<people>'";
            sql = sql.Replace("<IsRead>", IsRead).Replace("<topicid>", topicid).Replace("<people>", people);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static void UpdateTopicIsRead(string topicid, bool isread)
        {
            var IsRead = isread ? "TRUE" : "FALSE";
            var sql = "update auroratopicpeople set IsRead = '<IsRead>' where topicid= '<topicid>'";
            sql = sql.Replace("<IsRead>", IsRead).Replace("<topicid>", topicid);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static Dictionary<string, bool> RetrieveTopicIsRead(string people)
        {
            var ret = new Dictionary<string, bool>();
            var sql = "select topicid,IsRead from auroratopicpeople where people = '<people>'";
            sql = sql.Replace("<people>", people);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var topicid = Convert.ToString(line[0]);
                var IsRead = (string.Compare(Convert.ToString(line[1]),"TRUE") == 0)?true:false;
                if (!ret.ContainsKey(topicid))
                {
                    ret.Add(topicid, IsRead);
                }
            }
            return ret;
        } 

        public static List<string> GetNewPeopleList()
        {
            var ret = new List<string>();
            var sql = "select distinct people from auroranewpeople";
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                ret.Add(Convert.ToString(line[0]));
            }
            return ret;
        }

        public static List<CoTopicVM> RetrieveTopic4List(string username, string topicbelongtype,string status = "WORKING")
        {
            var topicreaddict = RetrieveTopicIsRead(username);

            var ret = new List<CoTopicVM>();
            var sql = "";
            if (string.Compare(topicbelongtype, TopicBelongType.IAssign) == 0)
            {
                sql = "select topicid,subject,creator,status,createdate from CoTopicVM where creator = '<creator>' and status = '<status>' and Removed <> 'TRUE' order by createdate desc";
                sql = sql.Replace("<creator>", username).Replace("<status>", status);
            }
            else
            {
                sql = "select topicid,subject,creator,status,createdate from CoTopicVM where topicid in (select distinct topicid from auroratopicpeople where people = '<people>' and isowner <> 'TRUE') and status = '<status>' and Removed <> 'TRUE'  order by createdate desc";
                sql = sql.Replace("<people>", username).Replace("<status>", status);
            }
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var tempvm = new CoTopicVM(Convert.ToString(line[0]), Convert.ToString(line[1]), "", Convert.ToString(line[2])
                    , Convert.ToString(line[3]), Convert.ToDateTime(line[4]).ToString("yyyy-MM-dd HH:mm:ss"));

                tempvm.eventtimelist = RetrieveTopicDueDate(tempvm.topicid);
                tempvm.ProjectWorkingList = TopicProject.RetrieveTopicPJ(tempvm.topicid, TopicPJStatus.Working);
                tempvm.ProjectDoneList = TopicProject.RetrieveTopicPJ(tempvm.topicid, TopicPJStatus.Done);
                if (topicreaddict.ContainsKey(tempvm.topicid))
                {
                    tempvm.isread = topicreaddict[tempvm.topicid];
                }

                ret.Add(tempvm);
            }

            return ret; 
        }

        public static List<CoTopicVM> RetrieveCompleteTopic4List(string username)
        {
            var Iassignlist = RetrieveTopic4List(username, TopicBelongType.IAssign, TopicStatus.Done);
            Iassignlist.AddRange(RetrieveTopic4List(username, TopicBelongType.IRelated, TopicStatus.Done));
            Iassignlist.Sort(delegate (CoTopicVM c1, CoTopicVM c2) {
                var date1 = DateTime.Parse(c1.createdate);
                var date2 = DateTime.Parse(c2.createdate);
                if (date2 > date1)
                    return 1;
                else if (date2 < date1)
                    return -1;
                else
                    return 0;
            });

            return Iassignlist;
        }

        public static List<CoTopicVM> RetrieveTopic(string topicid)
        {
            var ret = new List<CoTopicVM>();
            var sql = "select topicid,subject,creator,status,createdate,topiccontent from CoTopicVM where topicid = '<topicid>'";
            sql = sql.Replace("<topicid>", topicid);

            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var imgresize = SeverHtmlDecode.ResizeImageFromHtml(Convert.ToString(line[5]));
                
                var tempvm = new CoTopicVM(Convert.ToString(line[0]), Convert.ToString(line[1]), imgresize, Convert.ToString(line[2])
                    , Convert.ToString(line[3]), Convert.ToDateTime(line[4]).ToString("yyyy-MM-dd HH:mm:ss"));

                tempvm.eventtimelist = RetrieveTopicDueDate(tempvm.topicid);
                tempvm.ProjectWorkingList = TopicProject.RetrieveTopicPJ(tempvm.topicid, TopicPJStatus.Working);
                tempvm.ProjectDoneList = TopicProject.RetrieveTopicPJ(tempvm.topicid, TopicPJStatus.Done);
                tempvm.CommentList = TopicCommentVM.RetrieveComment(tempvm.topicid);

                ret.Add(tempvm);
            }

            return ret;
        }

        public CoTopicVM()
        {
            topicid = "";
            subject = "";
            topiccontent = "";
            creator = "";
            status = "";
            createdate = "";
            isread = true;
        }

        public CoTopicVM(string id,string sub,string content,string crtor,string stat,string cdate)
        {
            topicid = id;
            subject = sub;
            topiccontent = content;
            creator = crtor;
            status = stat;
            createdate = cdate;
        }

        private List<TopicCommentVM> comlist = new List<TopicCommentVM>();
        public List<TopicCommentVM> CommentList
        {
            set
            {
                comlist.Clear();
                comlist.AddRange(value);
            }
            get
            {
                return comlist;
            }
        }

        public string topicid { set; get; }
        public string subject { set; get; }
        public string topiccontent { set; get; }
        public string creator { set; get; }

        private List<EventTimer> evtlist = new List<EventTimer>();
        public List<EventTimer> eventtimelist {
            set { evtlist.Clear();
                evtlist.AddRange(value);
            }
            get { return evtlist; }
        }
        public string duedate { set; get; }
        public string status { set; get; }
        public string createdate { set; get; }
        public bool isread { set; get; }

        private List<string> prelatedpp = new List<string>();
        public List<string> relatedpeople {
            set {
                prelatedpp.Clear();
                prelatedpp.AddRange(value);
            }
            get {
                return prelatedpp;
            }
        }

        private List<TopicProject> pjwlist = new List<TopicProject>();
        public List<TopicProject> ProjectWorkingList {
            set {
                pjwlist.Clear();
                pjwlist.AddRange(value);
            }
            get {
                return pjwlist;
            }
        }


        private List<TopicProject> pjdlist = new List<TopicProject>();
        public List<TopicProject> ProjectDoneList
        {
            set{
                pjdlist.Clear();
                pjdlist.AddRange(value);
            }
            get{
                return pjdlist;
            }
        }


    }

    public class TopicProject
    {
        public TopicProject()
        {
            topicid = "";
            project = "";
            status = "";
            updater = "";
            updatedate = "";
        }

        public TopicProject(string id, string pj, string stat, string up, string uptime)
        {
            topicid = id;
            project = pj;
            status = stat;
            updater = up;
            updatedate = uptime;
        }

        public static void UpdateTopicPJ(string topicid, List<string> pjlist, Controller ctrl)
        {
            var sql = "delete from auroratopicpj where topicid = '<topicid>'";
            sql = sql.Replace("<topicid>", topicid);
            DBUtility.ExeLocalSqlNoRes(sql);

            foreach (var pj in pjlist)
            {
                sql = "insert into auroratopicpj(topicid,project,status,updatetime) values(@topicid,@project,@status,@updatetime)";
                var param = new Dictionary<string, string>();
                param.Add("@topicid",topicid);
                param.Add("@project",pj);
                param.Add("@status",TopicPJStatus.Working);
                param.Add("@updatetime",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                DBUtility.ExeLocalSqlNoRes(sql,param);
            }

            //var pjdict = CfgUtility.GetPJDict(ctrl);
            //foreach (var pj in pjlist)
            //{
            //    if (!pjdict.ContainsKey(pj))
            //    {
            //        sql = "delete from auroranewpj where project = '<project>'";
            //        sql = sql.Replace("<project>", pj);
            //        DBUtility.ExeLocalSqlNoRes(sql);

            //        sql = "insert into auroranewpj(project) values('<project>')";
            //        sql = sql.Replace("<project>", pj);
            //        DBUtility.ExeLocalSqlNoRes(sql);
            //    }
            //}
        }

        public static List<TopicProject> RetrieveTopicPJ(string topicid, string status)
        {
            var ret = new List<TopicProject>();
            var sql = "select topicid,project,status,updater,updatetime from auroratopicpj where topicid = '<topicid>' and status = '<status>'";
            sql = sql.Replace("<topicid>", topicid).Replace("<status>", status);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                ret.Add(new TopicProject(Convert.ToString(line[0]), Convert.ToString(line[1]), Convert.ToString(line[2])
                    , Convert.ToString(line[3]), Convert.ToDateTime(line[4]).ToString("yyyy-MM-dd HH:mm:ss")));
            }
            return ret;
        }

        public string topicid { set; get; }
        public string project { set; get; }
        public string status { set; get; }
        public string updater { set; get; }
        public string updatedate { set; get; }
    }
}