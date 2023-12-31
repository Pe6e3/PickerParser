﻿using Newtonsoft.Json;
using PickerParser.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        static int pageCount;
        List<Game> games = new List<Game>();
        private bool isDragging = false;
        private Point clickOffset;

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
        async Task ParseGameSlugs(int page)
        {
            string mainPage = await GetPageContent(baseUrl + "?page=" + page.ToString());
            string regexGame = @"<a href=""https://ru.pickgamer.com/games/(.*?)\/requirements""";
            MatchCollection matches = Regex.Matches(mainPage, regexGame);

            foreach (Match match in matches)
            {
                string gameUrl = match.Groups[1].ToString();
                if (!games.Any(x => x.GameUrl == gameUrl))
                    games.Add(new Game(gameUrl));


                if (gameInfoParseStatusBar.Value < gameInfoParseStatusBar.Maximum)
                {
                    gameInfoParseStatusBar.Value++;
                    int value = gameInfoParseStatusBar.Value;
                    int max = gameInfoParseStatusBar.Maximum;
                    gameInfoParseStatusBar.Text = $"{(value * 100) / max}% ({value.ToString()}/{max.ToString()}) -  {gameUrl}";
                }
                else
                    textStatusLabel.Text = "Слаги собраны, ждите...";
            }
        }

        private void Parser_Load(object sender, EventArgs e)
        {
            readJsonBtn.Click += (s, ev) => LoadJson();
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, "AllGames.json");
            if (File.Exists(filePath))
                readJsonBtn.Enabled = true;
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

        void SaveJson()
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filePath = Path.Combine(desktopPath, "AllGames.json"); // Путь к JSON-файлу
                string json = JsonConvert.SerializeObject(games);
                File.WriteAllText(filePath, json);

                MessageBox.Show($"JSON-файл с инфой об играх ({games.Count()} шт) сохранен на рабочем столе.\nПуть:  {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении JSON: {ex.Message}");
            }
        }   // Сохранить JSON файл
        void LoadJson()
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                string filePath = Path.Combine(desktopPath, "AllGames.json");

                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    games = JsonConvert.DeserializeObject<List<Game>>(json);
                    gamesCountLabel.Text = $"({games.Count.ToString()}) шт.";
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

        async void getLinksBtn_Click(object sender, EventArgs e)
        {
            if (int.TryParse(pageFromField.Text, out int pageFrom) && int.TryParse(pageToField.Text, out int pageTo)) // Проверяем, что в полях целые числа
            {
                if (pageFrom <= pageTo && pageTo <= pageCount)
                {
                    games.Clear();
                    gameInfoParseStatusBar.Value = 1;
                    gameInfoParseStatusBar.Maximum = (pageTo - pageFrom + 1) * 20;
                    textStatusLabel.Text = "Получение Слагов игр";
                    try
                    {
                        for (int i = pageFrom; i <= pageTo; i++)
                            await ParseGameSlugs(i);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при парсинге слагов игр: " + ex.Message);
                    }
                    await GetGames();
                }
            }
        }  // Получить Слаги игр

        async void getPageCount_Click(object sender, EventArgs e)
        {
            await GetPageCountAsync();
            pageToField.Enabled = true;
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

        private void exitBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Parser_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                clickOffset = e.Location;
            }
        }

        private void Parser_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                isDragging = false;
        }

        private void Parser_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point newLocation = this.Location;
                newLocation.X += e.X - clickOffset.X;
                newLocation.Y += e.Y - clickOffset.Y;
                this.Location = newLocation;
            }
        }

        async Task GetGames()
        {
            textStatusLabel.Text = "Получение данных об играх";
            gameInfoParseStatusBar.Value = 1;
            gameInfoParseStatusBar.Maximum = games.Count;


            List<Task> parsingTasks = new List<Task>();    // список задач для парсинга данных об играх

            try
            {
                foreach (var game in games)                // парсинг каждой игры и добавление задачи в список
                    parsingTasks.Add(ParseGame(game));

                await Task.WhenAll(parsingTasks);          // ждем завершения всех задач парсинга
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при парсинге данных об играх: " + ex.Message);
            }

            SaveJson();
            LoadJson();
            textStatusLabel.Text = "";
        }

        async Task ParseGame(Game game)
        {
            try
            {

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

                // обновляем прогресс бар в главном потоке, не блокируя остальные потоки
                gameInfoParseStatusBar.BeginInvoke((Action)(() =>
                {
                    if (gameInfoParseStatusBar.Value < gameInfoParseStatusBar.Maximum)
                    {
                        gameInfoParseStatusBar.Value++;
                        int value = gameInfoParseStatusBar.Value;
                        int max = gameInfoParseStatusBar.Maximum;
                        gameInfoParseStatusBar.Text = $"{(value * 100) / max}% ({value}/{max})  -  {game.GameName}";
                    }
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при парсинге игры {game.GameName}: {ex.Message}");
            }
        }

    }
}