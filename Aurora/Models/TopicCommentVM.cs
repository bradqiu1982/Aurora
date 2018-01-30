using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aurora.Models
{
    public class TopicCommentVM
    {
        public TopicCommentVM()
        {
            topicid = "";
            commentid = "";
            commentcontent = "";
            creator = "";
            commentdate = "";
        }

        public TopicCommentVM(string tid,string cid,string cc,string crter,string ct)
        {
            topicid = tid;
            commentid = cid;
            commentcontent = cc;
            creator = crter;
            commentdate = ct;
        }

        public static void AddComment(string tid, string cid, string cc, string crter, string cdate)
        {
            var sql = "insert into TopicCommentVM(topicid,commentid,commentcontent,creator,commentdate) values(@topicid,@commentid,@commentcontent,@creator,@commentdate)";
            var param = new Dictionary<string,string>();
            param.Add("@topicid",tid);
            param.Add("@commentid", cid);
            param.Add("@commentcontent", cc);
            param.Add("@creator", crter);
            param.Add("@commentdate", cdate);
            DBUtility.ExeLocalSqlNoRes(sql, param);
        }

        public static List<TopicCommentVM> RetrieveComment(string tid)
        {
            var ret = new List<TopicCommentVM>();
            var sql = "select topicid,commentid,commentcontent,creator,commentdate from TopicCommentVM where topicid = '<topicid>'";
            sql = sql.Replace("<topicid>", tid);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var imgresize = SeverHtmlDecode.ResizeImageFromHtml(Convert.ToString(line[2]));
                ret.Add(new TopicCommentVM(Convert.ToString(line[0]), Convert.ToString(line[1])
                    , imgresize, Convert.ToString(line[3]), Convert.ToDateTime(line[4]).ToString("yyyy-MM-dd HH:mm:ss")));
            }
            return ret;
        }

        public static void UpdateComment(string cid,string cc)
        {
            var sql = "update TopicCommentVM set commentcontent = @commentcontent where commentid = @commentid";
            var param = new Dictionary<string, string>();
            param.Add("@commentid", cid);
            param.Add("@commentcontent", cc);
            DBUtility.ExeLocalSqlNoRes(sql, param);
        }

        public string topicid { set; get; }
        public string commentid { set; get; }
        public string commentcontent { set; get; }
        public string creator { set; get; }
        public string commentdate { set; get; }

    }
}