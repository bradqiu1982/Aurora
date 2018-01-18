using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aurora.Models
{
    public class Theme
    {
        public static int Dark = 1;
        public static int Light = 2;
    }
    public class UserCustomizeThemeVM
    {
        public UserCustomizeThemeVM()
        {
            ID = 0;
            MachineName = string.Empty;
            Theme = 1;
            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public UserCustomizeThemeVM(int id, string machine, int theme, string ctime, string utime)
        {
            ID = id;
            MachineName = machine;
            Theme = theme;
            CreateTime = ctime;
            UpdateTime = utime;
        }

        public int ID { set; get; }
        public string MachineName { set; get; }
        public int Theme { set; get; }
        public string CreateTime { set; get; }
        public string UpdateTime { set; get; }

        public static void UpdateTheme(UserCustomizeThemeVM ctheme)
        {
            var UserTheme = GetUserTheme(ctheme.MachineName);
            if (UserTheme.ID > 0)
            {
                UpdateUserTheme(ctheme);
            }
            else
            {
                CreateUserTheme(ctheme);
            }
        }

        public static UserCustomizeThemeVM GetUserTheme(string machine)
        {
            var sql = "select ID, MachineName, Theme, CreateTime, UpdateTime "
                    + "from UserCustomizeTheme "
                    + "where MachineName = '<MachineName>'";
            sql = sql.Replace("<MachineName>", machine);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            var ret = new UserCustomizeThemeVM();
            if (dbret.Count > 0)
            {
                ret.ID = Convert.ToInt32(dbret[0][0]);
                ret.MachineName = Convert.ToString(dbret[0][1]);
                ret.Theme = Convert.ToInt32(dbret[0][2]);
                ret.CreateTime = Convert.ToString(dbret[0][3]);
                ret.UpdateTime = Convert.ToString(dbret[0][4]);
            }
            return ret;
        }

        public static void UpdateUserTheme(UserCustomizeThemeVM ctheme)
        {
            var sql = "update UserCustomizeTheme set "
                    + "Theme = '<Theme>', UpdateTime = '<UpdateTime>' "
                    + "where MachineName = '<MachineName>'";
            sql = sql.Replace("<Theme>", ctheme.Theme.ToString())
                    .Replace("<UpdateTime>", ctheme.UpdateTime)
                    .Replace("<MachineName>", ctheme.MachineName);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static void CreateUserTheme(UserCustomizeThemeVM ctheme)
        {
            var sql = "insert into UserCustomizeTheme "
                    + "(MachineName, Theme, CreateTime, UpdateTime) "
                    + "values ('<MachineName>', '<Theme>', '<CreateTime>', '<UpdateTime>')";

            sql = sql.Replace("<MachineName>", ctheme.MachineName)
                    .Replace("<Theme>", ctheme.Theme.ToString())
                    .Replace("<CreateTime>", ctheme.CreateTime)
                    .Replace("<UpdateTime>", ctheme.UpdateTime);
            DBUtility.ExeLocalSqlNoRes(sql);
        }
    }
}