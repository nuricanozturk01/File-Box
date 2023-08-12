using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    internal class TurkishhLanguage : ILanguage
    {
        public string getButtonName()
        {
            return "BANA TIKLA";
        }

        public string getLabelName()
        {
            return "ANAN";
        }
    }
}
