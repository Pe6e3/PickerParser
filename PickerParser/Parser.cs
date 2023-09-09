using Newtonsoft.Json;
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

        static string baseUrl = "https://ru.pickgamer.com/games";
        static List<Game> games = new List<Game>();

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

        static async Task<string> GetPageContent(string url)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                return await wc.DownloadStringTaskAsync(url);
            }
        }
        static async Task ParseGameNames()
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


        async Task GetGamesInfo()
        {
            int count = 0; ////////////////////////////////////
            foreach (var game in games)
            {
                if (++count == 5) return; ///////////////////////

                string gamePage = await GetPageContent($"{baseUrl}/{game.GameUrl}/requirements");
                string regexRequirements = @"<li>(.*?)\s*:\s*(.*?)<\/li>";
                string regexGameName = "<h2>Вот такие системные требования <em>(.*?)</em>";
                MatchCollection maches = Regex.Matches(gamePage, regexRequirements);
                Match matchGameName = Regex.Match(gamePage, regexGameName);
                game.GameName = matchGameName.Groups[1].Value;
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
                Game gameFromList = games.FirstOrDefault(x => x.GameName == game.GameName);
                gameFromList = game;  // вместо  Update(gameFromList);




            }

        }

        static async void SaveAllGamesJson()
        {
            int count = 0; ////////////////////////////////////
            foreach (var game in games)
            {
                if (++count == 5) return; ///////////////////////

                string json = JsonConvert.SerializeObject(game);
                SaveJsonOnDesktop(game.GameUrl, json);
            }
        }

        async void button2_Click(object sender, EventArgs e)
        {
            await GetGamesInfo();
            RefreshGamesCB();
        }



        public static void SaveJsonOnDesktop(string fileName, string json)
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string folderPath = Path.Combine(desktopPath, "1"); // Путь к папке
                Directory.CreateDirectory(folderPath); // Создаем папку, если она не существует
                string filePath = Path.Combine(folderPath, fileName + ".json"); // Путь к файлу внутри папки


                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении JSON: {ex.Message}");
            }
        }

        public void LoadAndDisplayJson(string fileName)
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string folderPath = Path.Combine(desktopPath, "1"); // Путь к папке
                Directory.CreateDirectory(folderPath); // Создаем папку, если она не существует
                string filePath = Path.Combine(folderPath, fileName + ".json"); // Путь к файлу внутри папки


                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    Game gameData = JsonConvert.DeserializeObject<Game>(json);


                    label2.Text = "Game Name: " + gameData.GameName;
                    label3.Text = "Game URL: " + gameData.GameUrl;
                    label4.Text = "CPU: " + gameData.minRequirements.CPU;
                    label5.Text = "RAM: " + gameData.minRequirements.RAM;
                    label6.Text = "OS: " + gameData.minRequirements.OS;
                    label7.Text = "Videocard: " + gameData.minRequirements.Videocard;
                    label8.Text = "Pixel: " + gameData.minRequirements.Pixel;
                    label9.Text = "Vertex: " + gameData.minRequirements.Vertex;
                    label10.Text = "Disk Space: " + gameData.minRequirements.DiskSpace;
                    label11.Text = "Video RAM: " + gameData.minRequirements.VideoRam;
                }
                else
                    MessageBox.Show("Файл не существует.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке и отображении JSON: {ex.Message}");
            }
        }

        private void loadJson_Click(object sender, EventArgs e)
        {
            LoadAndDisplayJson(jsonFileName.Text.ToString());
        }

        void RefreshGamesCB()
        {
            gamesCB.DataSource = games;
            gamesCB.DisplayMember = "GameName";
        }

        private void saveJsonBtn_Click(object sender, EventArgs e)
        {
            SaveAllGamesJson();
        }
    }
}
