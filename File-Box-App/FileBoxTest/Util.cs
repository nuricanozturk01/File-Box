using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBoxTest
{
    public static class Util
    {
        public static string CreateRandomFolderName(this Random random)
        {
            return Path.GetRandomFileName();
        }
    }
}
