using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentGatherer1
{
    class Class1
    {
        public string path;
        public int filesize;
        public string type;

        Class1(string p, int s, string t)
        {
            path = p;
            filesize = s;
            type = t;
        }
    }
}
