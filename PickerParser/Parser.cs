using PickerParser.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PickerParser
{
    public partial class Parser : Form
    {

        string baseUrl = "https://ru.pickgamer.com/games";
        List<Game> games = new List<Game>();

        public Parser()
        {
            InitializeComponent();
        }

        async void button1_Click(object sender, EventArgs e)
        {

            await ParseGameNames();
            foreach (var game in games)
                output.Text += game.GameUrl + "\n";
        }

        async Task<string> GetPageContent(string url)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                return await wc.DownloadStringTaskAsync(url);
            }
        }
        async Task ParseGameNames()
        {
            string mainPage = await GetPageContent(baseUrl);
            string regexGame = @"<a href=""https://ru.pickgamer.com/games/(.*?)\/requirements""";
            MatchCollection matches = Regex.Matches(mainPage, regexGame);

            foreach (Match match in matches)
            {
                string gameUrl = match.Groups[1].ToString();
                if (!games.Any(x => x.GameUrl == gameUrl))
                    games.Add(new Game(gameUrl));
            }

        }


        async Task ParseRequirements(Game game)
        {
            string gamePage = await GetPageContent($"{baseUrl}/{game.GameUrl}/requirements");
            string regexRequirements = @"<li>(.*?)\s*:\s*(.*?)<\/li>";
            string regexGameName = "<h2>Вот такие системные требования <em>(.*?)</em>";
            MatchCollection maches = Regex.Matches(gamePage, regexRequirements);
            Match matchGameName = Regex.Match(gamePage, regexGameName);
            game.GameName= matchGameName.Groups[1].Value;
            foreach (Match match in maches)
            {
                string reqName = match.Groups[1].Value;
                string reqValue = match.Groups[2].Value;

                switch (reqName)
                {
                    case "ПРОЦЕССОР": game.minRequirements.CPU = reqValue; break;
                    case "ОПЕРАТИВНАЯ ПАМЯТЬ": game.minRequirements.RAM = reqValue; break;
                    case "ОС": game.minRequirements.OS = reqValue; break;
                    case "ВИДЕОКАРТА": game.minRequirements.Videocard = reqValue; break;
                    case "PIXEL ШЕЙДЕРЫ": game.minRequirements.Pixel = reqValue; break;
                    case "VERTEX ШЕЙДЕРЫ": game.minRequirements.Vertex = reqValue; break;
                    case "СВОБОДНОЕ МЕСТО НА ДИСКЕ": game.minRequirements.DiskSpace = reqValue; break;
                    case "ВЫДЕЛЕННАЯ ВИДЕО ПАМЯТЬ": game.minRequirements.VideoRam = reqValue; break;
                    default: break;
                }
            }

            label1.Text =
                game.GameName + "\n" +
                game.GameUrl + "\n" +
                game.minRequirements.CPU + "\n" +
                game.minRequirements.RAM + "\n" +
                game.minRequirements.OS + "\n" +
                game.minRequirements.Videocard + "\n" +
                game.minRequirements.Pixel + "\n" +
                game.minRequirements.Vertex + "\n" +
                game.minRequirements.DiskSpace + "\n" +
                game.minRequirements.VideoRam;

        }
        async void button2_Click(object sender, EventArgs e)
        {
            Game game = games.Skip(5).Take(1).FirstOrDefault();
            await ParseRequirements(game);
        }

    }
}
