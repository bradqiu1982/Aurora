using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using RDotNet;
using System.Collections;
using Aurora.Models;

namespace Aurora.Controllers
{
    public class MainController : Controller
    {
        // GET: Main
        public ActionResult Index()
        {

                //REngine.SetEnvironmentVariables();
                REngine.SetEnvironmentVariables("C:/Rtools/R/R342/bin/i386", "C:/Rtools/R/R342");
                // There are several options to initialize the engine, but by default the following suffice:
                REngine engine = REngine.GetInstance();

            //smaple 1 -- use statics function
            //engine.Evaluate("library(stats)");
            //engine.Evaluate("options(digits=2)");
            //var colname = new string[] { "Student","Math","Science","English" };
            //var Student = new string[] { "john dav","angela willia","bullwink moose","david jone","janice mark","chery cush","reuven ytz","greg knox","joel england","mary ray"};
            //var Math = new int[] { 502,600,412,358,495,512,410,625,573,522};
            //var Science = new int[] { 95,99,80,82,75,85,80,95,89,86};
            //var English = new int[] { 25,22,18,15,20,28,15,30,27,18};
            //IEnumerable[] srcdata = new IEnumerable[4];
            //srcdata[0] = Student;
            //srcdata[1] = Math;
            //srcdata[2] = Science;
            //srcdata[3] = English;
            //var roster = engine.CreateDataFrame(srcdata, colname);

            //engine.SetSymbol("roster", roster);
            //engine.Evaluate("z <- scale(roster[,2:4])");
            //engine.Evaluate("score <- apply(z,1,mean)");
            //engine.Evaluate("roster <- cbind(roster,score)");
            //engine.Evaluate("y <- quantile(score, c(.8,.6,.4,.2))");
            //engine.Evaluate("roster$grad[score >= y[1]] <- \"A\"");
            //engine.Evaluate("roster$grad[score < y[1] & score >= y[2]] <- \"B\"");
            //engine.Evaluate("roster$grad[score < y[2] & score >= y[3]] <- \"C\"");
            //engine.Evaluate("roster$grad[score < y[3] & score >= y[4]] <- \"D\"");
            //engine.Evaluate("roster$grad[score < y[4]] <- \"F\"");
            //engine.Evaluate("roster <- roster[order(roster$Student),]");

            //roster = engine.GetSymbol("roster").AsDataFrame();

            //var colnum = roster.ColumnCount;
            //var rows =  roster.GetRows();
            //foreach (DataFrameRow r in rows)
            //{
            //    var vlist = new List<object>();
            //    for (var cl = 0; cl < colnum; cl++)
            //    {
            //        vlist.Add(r[cl]);
            //    }
            //    System.Windows.MessageBox.Show(string.Join(",", vlist));
            //}

            //sample2 -- generate plot
            var plotfolder = this.Server.MapPath("~/userfiles") + "/docs/plot/";
            CreateFolder(plotfolder);
            var fn = Guid.NewGuid().ToString("N") + ".png";
            var plotname = plotfolder + fn;
            var url = "/userfiles/docs/plot/" + fn;

            var does = new double[] { 20, 30, 40, 45, 60 };
            var druga = new double[] { 16, 20, 27, 40, 60 };
            var cdoes = engine.CreateNumericVector(does);
            var cdruga = engine.CreateNumericVector(druga);

            engine.Evaluate("library(graphics)");
            engine.SetSymbol("does", cdoes);
            engine.SetSymbol("draga", cdruga);
            engine.Evaluate("png('" + plotname.Replace("\\", "/") + "')");
            engine.Evaluate("plot(does,draga,type='b')");
            engine.Evaluate("dev.off()");
            engine.ClearGlobalEnvironment();
            ViewBag.url = url;

            return View();
        }

        private void CreateFolder(string folder)
        {
            if (!System.IO.Directory.Exists(folder))
            { System.IO.Directory.CreateDirectory(folder); }
        }
    }
}