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
        static int pageCount;

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
        static async Task ParseGameSlugs(int page)
        {
            string mainPage = await GetPageContent(baseUrl + "?page=" + page.ToString());
            string regexGame = @"<a href=""https://ru.pickgamer.com/games/(.*?)\/requirements""";
            MatchCollection matches = Regex.Matches(mainPage, regexGame);

            foreach (Match match in matches)
            {
                string gameUrl = match.Groups[1].ToString();
                if (!games.Any(x => x.GameUrl == gameUrl))
                    games.Add(new Game(gameUrl));
            }
        }

        void RefreshGamesCB()
        {
            gamesCB.DataSource = games;
            gamesCB.DisplayMember = "GameName";
        }
        void gamesCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gamesCB.SelectedItem is Game game)
            {
                UrlLabel.Text = game.GameUrl;
                minCpuLabel.Text = game.minRequirements.CPU;
                minRamLabel.Text = game.minRequirements.RAM;
                minOsLabel.Text = game.minRequirements.OS;
                minVideoLabel.Text = game.minRequirements.Videocard;
                minPixelLabel.Text = game.minRequirements.Pixel;
                minVertexLabel.Text = game.minRequirements.Vertex;
                minSpaceLabel.Text = game.minRequirements.DiskSpace;
                minVideoRamLabel.Text = game.minRequirements.VideoRam;

                optCpuLabel.Text = game.optRequirements.CPU;
                optRamLabel.Text = game.optRequirements.RAM;
                optOsLabel.Text = game.optRequirements.OS;
                optVideoLabel.Text = game.optRequirements.Videocard;
                optPixelLabel.Text = game.optRequirements.Pixel;
                optVertexLabel.Text = game.optRequirements.Vertex;
                optSpaceLabel.Text = game.optRequirements.DiskSpace;
                optVideoRamLabel.Text = game.optRequirements.VideoRam;
            }
        }  // Заполняем поля с инфой о выделенной игре


        void saveJsonBtn_Click(object sender, EventArgs e)
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
        void readJsonBtn_Click(object sender, EventArgs e)
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
                    gamesCountLabel.Text = games.Count.ToString() + " шт.";
                    MessageBox.Show("Данные о всех играх успешно загружены и могут быть обработаны.");
                }
                else
                    MessageBox.Show("Файл AllGames.json не существует.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке и отображении JSON: {ex.Message}");
            }

            RefreshGamesCB();
        }   // Перенести из JSON файла все игры в games (List<Game>)
        async void getGamesInfoBtn_Click(object sender, EventArgs e)
        {

            gameInfoParseStatusBar.Value = 1;
            gameInfoParseStatusBar.Maximum = games.Count;

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

                string gamePage = await GetPageContent($"{baseUrl}/{game.GameUrl}/requirements");
                string regexRequirements = @"<li>(.*?)\s*:\s*(.*?)<\/li>";
                string regexGameName = "<h2>Вот такие системные требования <em>(.*?)</em>";
                MatchCollection maches = Regex.Matches(gamePage, regexRequirements);
                Match matchGameName = Regex.Match(gamePage, regexGameName);
                game.GameName = matchGameName.Groups[1].Value;
                bool min = true;
                foreach (Match match in maches)
                {
                    string reqName = match.Groups[1].Value;
                    string reqValue = match.Groups[2].Value;

                    if (min)  // Проверяем - парсим еще минимальные требования или уже оптимальные? (они идут попорядку)
                    {
                        switch (reqName)
                        {
                            case "ПРОЦЕССОР": game.minRequirements.CPU = reqValue; break;
                            case "ОПЕРАТИВНАЯ ПАМЯТЬ": game.minRequirements.RAM = reqValue; break;
                            case "ОС": game.minRequirements.OS = reqValue; break;
                            case "ВИДЕОКАРТА": game.minRequirements.Videocard = reqValue; break;
                            case "PIXEL ШЕЙДЕРЫ": game.minRequirements.Pixel = reqValue; break;
                            case "VERTEX ШЕЙДЕРЫ": game.minRequirements.Vertex = reqValue; break;
                            case "СВОБОДНОЕ МЕСТО НА ДИСКЕ": game.minRequirements.DiskSpace = reqValue; break;
                            case "ВЫДЕЛЕННАЯ ВИДЕО ПАМЯТЬ": game.minRequirements.VideoRam = reqValue; min = false; break; // Ставим метку, что все минимальные требования спарсили
                            default: break;
                        }
                    }
                    else
                    {
                        switch (reqName)
                        {
                            case "ПРОЦЕССОР": game.optRequirements.CPU = reqValue; break;
                            case "ОПЕРАТИВНАЯ ПАМЯТЬ": game.optRequirements.RAM = reqValue; break;
                            case "ОС": game.optRequirements.OS = reqValue; break;
                            case "ВИДЕОКАРТА": game.optRequirements.Videocard = reqValue; break;
                            case "PIXEL ШЕЙДЕРЫ": game.optRequirements.Pixel = reqValue; break;
                            case "VERTEX ШЕЙДЕРЫ": game.optRequirements.Vertex = reqValue; break;
                            case "СВОБОДНОЕ МЕСТО НА ДИСКЕ": game.optRequirements.DiskSpace = reqValue; break;
                            case "ВЫДЕЛЕННАЯ ВИДЕО ПАМЯТЬ": game.optRequirements.VideoRam = reqValue; break;
                            default: break;
                        }
                    }
                }

                if (gameInfoParseStatusBar.Value < gameInfoParseStatusBar.Maximum)
                {
                    gameInfoParseStatusBar.Value++;
                    parseStatusLabel.Text = gameInfoParseStatusBar.Value.ToString() + "/" + gameInfoParseStatusBar.Maximum.ToString() + "   " + game.GameUrl;
                }
                else
                {
                    saveJsonBtn.Enabled = true;
                    getGamesInfoBtn.Enabled = false;
                }
            }
        }  // Парсит данные по каждой игре
        async void getLinksBtn_Click(object sender, EventArgs e)
        {
            if (int.TryParse(pageFromField.Text, out int pageFrom) && int.TryParse(pageToField.Text, out int pageTo))
            {
                if (pageFrom <= pageTo && pageTo <= pageCount)
                {
                    games.Clear();
                    gamesList.Items.Clear();
                    for (int i = pageFrom; i <= pageTo; i++)
                        await ParseGameSlugs(i);

                    foreach (Game game in games)
                        gamesList.Items.Add(game.GameUrl);
                    getGamesInfoBtn.Enabled = true;
                }
            }
        }  // Получить Слаги игр


        async void getPageCount_Click(object sender, EventArgs e)
        {
            await GetPageCountAsync();
        }

        async Task GetPageCountAsync()
        {
            string pagexRegex = "page=(.*?)>(?<page>.*?)</a></li>";
            string mainPage = await GetPageContent(baseUrl);
            MatchCollection matches = Regex.Matches(mainPage, pagexRegex);
            pageToField.Text = matches[matches.Count - 1].Groups["page"].ToString();
            pageCount = Convert.ToInt32(pageToField.Text);
            getLinksBtn.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
