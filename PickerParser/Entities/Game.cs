﻿namespace PickerParser.Entities
{
    public class Game
    {
        public Game(string gameName)
        {
            GameUrl = gameName;
            minRequirements = new Requirements();
            recRequirements = new Requirements();
        }
        public Game()
        {

        }
        public string GameName { get; set; }
        public string GameUrl { get; set; }
        public Requirements minRequirements { get; set; }
        public Requirements recRequirements { get; set; }
    }
}
