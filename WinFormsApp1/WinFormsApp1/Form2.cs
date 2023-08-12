
namespace WinFormsApp1
{
    public partial class Form2 : Form
    {
        Form form1;
        public Form2()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if (checkBox1.Checked)
            {
                form1 = new Form1(new TurkishhLanguage());
            }
            else form1 = new Form1(new EnglishLanguage());

            form1.Show();
        }
    }
}
