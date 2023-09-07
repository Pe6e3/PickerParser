using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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

        private void button1_Click(object sender, EventArgs e)
        {
            
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                pageContent = wc.DownloadString(baseUrl);
            }

         

        }

        async void ParseGameNames()
        {
            string regexGame = @"<a href=""https://ru.pickgamer.com/games/(.*?)\/requirements""";
            MatchCollection matches = Regex.Matches(pageContent, regexGame);

            List<string> games = new List<string>();
            foreach (Match match in matches)
                games.Add(match.Groups[1].ToString());
        }


    }
}
