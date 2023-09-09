namespace PickerParser.Entities
{
    public class Game
    {
        public Game(string gameName)
        {
            GameUrl = gameName;
            minRequirements = new Requirements();
            optRequirements = new Requirements();
        }
        public Game()
        {
            minRequirements = new Requirements();
            optRequirements = new Requirements();
        }
        public string GameName { get; set; }
        public string GameUrl { get; set; }
        public Requirements minRequirements { get; set; }
        public Requirements optRequirements { get; set; }
    }
}
