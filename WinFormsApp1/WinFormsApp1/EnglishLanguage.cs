using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    internal class EnglishLanguage : ILanguage
    {
        public string getButtonName()
        {
            return "Click me";
        }

        public string getLabelName()
        {
            return "Your mother";
        }
    }
}
