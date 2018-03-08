using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Aurora.Models;

namespace Aurora.Controllers
{
    public class TechnicalVideoController : Controller
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
            ViewBag.compName = compName.ToUpper();
            var glbcfg = CfgUtility.GetSysConfig(this);

            var usermap = MachineUserMap.RetrieveUserMap();
            foreach (var item in usermap)
            {
                if (!glbcfg.ContainsKey(item.machine))
                {
                    glbcfg.Add(item.machine, item.username);
                }
            }

            if (glbcfg.ContainsKey(ViewBag.compName))
            {
                ViewBag.username = glbcfg[ViewBag.compName].Trim().ToUpper();
            }
            else
            {
                ViewBag.username = ViewBag.compName;
            }
        }


        // GET: TechnicalVideo
        public ActionResult Home()
        {
            return View();
        }

        private List<string> GetVideoFile()
        {
            var ret = new List<string>();
            try
            {
                foreach (string fl in Request.Files)
                {
                    if (fl != null && Request.Files[fl].ContentLength > 0)
                    {
                        var allvtype = "mp4,mp3,h264,wmv,wav,avi,flv,mov,mkv,webm,ogg";
                        var ext = Path.GetExtension(Request.Files[fl].FileName).ToLower().Replace(".","").Trim();
                        if (allvtype.Contains(ext))
                        {
                            string datestring = DateTime.Now.ToString("yyyyMMdd");
                            string imgdir = Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
                            if (!Directory.Exists(imgdir))
                            {
                                Directory.CreateDirectory(imgdir);
                            }

                            var fn = Path.GetFileName(Request.Files[fl].FileName)
                            .Replace(" ", "_").Replace("#", "")
                            .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");
                            fn = Path.GetFileNameWithoutExtension(fn) + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(fn);
                            var onlyname = Path.GetFileNameWithoutExtension(fn);

                            var srcvfile = imgdir + fn;
                            //store file to local
                            Request.Files[fl].SaveAs(srcvfile);

                            var imgname = onlyname + ".jpg";
                            var imgpath = imgdir + imgname;
                            var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                            ffMpeg.GetVideoThumbnail(srcvfile, imgpath);

                            var oggname = onlyname + ".ogg";
                            var oggpath = imgdir + oggname;
                            var ffMpeg1 = new NReco.VideoConverter.FFMpegConverter();
                            ffMpeg1.ConvertMedia(srcvfile, oggpath, NReco.VideoConverter.Format.ogg);

                            if (!ext.Contains("mp4"))
                            {
                                var mp4name = onlyname + ".mp4";
                                var mp4path = imgdir + mp4name;
                                var ffMpeg2 = new NReco.VideoConverter.FFMpegConverter();
                                ffMpeg2.ConvertMedia(srcvfile, mp4path, NReco.VideoConverter.Format.mp4);
                            }

                            var url = "/userfiles/docs/" + datestring + "/" + onlyname;
                            ret.Add(url);
                        }//end if ext
                    }//file is not null
                }//foreach
            }
            catch (Exception ex)
            { ret.Clear(); }

            return ret;
        }

        public ActionResult UploadVideo()
        {
            UserAuth();
            var videolist = GetVideoFile();
            if (videolist.Count > 0)
            {

            }

            return RedirectToAction("Home", "TechnicalVideo");
        }

        public JsonResult UploadVideoHash()
        {
            var ret = new JsonResult();
            ret.Data = new { success = true };
            return ret;
        }

    }
}