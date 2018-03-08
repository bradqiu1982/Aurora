using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aurora.Models
{
    public class VideoVM
    {
        public static string GetUniqKey()
        {
            return Guid.NewGuid().ToString("N");
        }

    }
}