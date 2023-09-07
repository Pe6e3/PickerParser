using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PickerParser
{
    public partial class Form1 : Form
    {

        string baseUrl = "https://ru.pickgamer.com/games";
        string pageContent = "";
        public Form1()
        {
            InitializeComponent();
        }

        async void button1_Click(object sender, EventArgs e)
        {
            await GetPageContent();
            ParseGameNames();
        }

        async Task GetPageContent()
        {
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                pageContent = await wc.DownloadStringTaskAsync(baseUrl);
            }
        }
        void ParseGameNames()
        {
            string regexGame = @"<a href=""https://ru.pickgamer.com/games/(.*?)\/requirements""";
            MatchCollection matches = Regex.Matches(pageContent, regexGame);

            HashSet<string> games = new HashSet<string>();
            foreach (Match match in matches)
                games.Add(match.Groups[1].ToString());

            output.Text = string.Join("\n", games); ///////////////////////
        }

    }
}
