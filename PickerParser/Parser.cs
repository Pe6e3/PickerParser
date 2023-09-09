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

        static async Task<string> GetPageContent(string url)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                return await wc.DownloadStringTaskAsync(url);
            }
        }
        static async Task ParseGameSlugs()
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

        public void LoadAllGamesJson()
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string folderPath = Path.Combine(desktopPath, "1"); // Путь к папке
                string filePath = Path.Combine(folderPath, "AllGames.json");

                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    games = JsonConvert.DeserializeObject<List<Game>>(json);
                    MessageBox.Show("Данные о всех играх успешно загружены и могут быть обработаны.");
                }
                else
                    MessageBox.Show("Файл AllGames.json не существует.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке и отображении JSON: {ex.Message}");
            }
        }

        void RefreshGamesCB()
        {
            gamesCB.DataSource = games;
            gamesCB.DisplayMember = "GameName";
        }

        private void saveJsonBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string folderPath = Path.Combine(desktopPath, "1"); // Путь к папке
                Directory.CreateDirectory(folderPath); // Создаем папку, если она не существует
                string filePath = Path.Combine(folderPath, "AllGames.json"); // Путь к JSON-файлу

                string json = JsonConvert.SerializeObject(games);
                File.WriteAllText(filePath, json);

                MessageBox.Show("JSON-файл со всеми играми успешно сохранен.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении JSON: {ex.Message}");
            }
        }   // Сохранить JSON файл

        private void gamesCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gamesCB.SelectedItem is Game game)
            {
                gameNameLabel.Text = game.GameName;
                UrlLabel.Text = game.GameUrl;
                minCpuLabel.Text = game.minRequirements.CPU;
                minRamLabel.Text = game.minRequirements.RAM;
                minOsLabel.Text = game.minRequirements.OS;
                minVideoLabel.Text = game.minRequirements.Videocard;
                nimPixelLabel.Text = game.minRequirements.Pixel;
                minVertexLabel.Text = game.minRequirements.Vertex;
                minSpaceLabel.Text = game.minRequirements.DiskSpace;
                minVideoRamLabel.Text = game.minRequirements.VideoRam;
            }
        }  // Заполняем поля с инфой о выделенной игре

        async void getLinksBtn_Click(object sender, EventArgs e)
        {
            await ParseGameSlugs();

            gamesList.Items.Clear();
            foreach (Game game in games)
                gamesList.Items.Add(game.GameUrl);
        }  // Получить Слаги игр

        private void readJsonBtn_Click(object sender, EventArgs e)
        {
            LoadAllGamesJson();
            RefreshGamesCB();
        }   // Перенести из JSON файла все игры в games (List<Game>)

        async void getGamesInfoBtn_Click(object sender, EventArgs e)
        {

            gameInfoParseStatusBar.Maximum = 4;
            gameInfoParseStatusBar.Value = 1;
            //gameInfoParseStatusBar.Maximum = games.Count;

            int count = 0; ////////////////////////////////////
            foreach (var game in games)
            {
                foreach (ListViewItem item in gamesList.Items) // Выделяет в списке игру, которая сейчас обрабатывается (для визуализации)
                {
                    item.Selected = false;
                    if (item.Text == game.GameUrl)
                    {
                        item.Selected = true;
                        gamesList.Select();
                        break; // Чтобы не продолжать поиск после нахождения совпадения
                    }
                };

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

                if (gameInfoParseStatusBar.Value < gameInfoParseStatusBar.Maximum)
                    gameInfoParseStatusBar.Value++;
            }
        }  // Парсит данные по каждой игре
    }
}
