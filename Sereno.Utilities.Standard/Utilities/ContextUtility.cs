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
            Context result = new Context()
            {
                UserName = ""
            };

            return result;
        }

        public static Context Create(string userName)
        {
            Context result = new Context()
            {
                UserName = userName,
            };

            return result;
        }
    }
}
