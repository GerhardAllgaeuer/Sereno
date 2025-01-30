using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sereno.Utilities
{
    public class ContextUtility
    {
        public static Context Create()
        {
            Context result = new()
            {
                UserName = ""
            };

            return result;
        }

        public static Context Create(string userName)
        {
            Context result = new()
            {
                UserName = userName,
            };

            return result;
        }
    }
}
