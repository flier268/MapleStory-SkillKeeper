using System.Collections.ObjectModel;
using MapleStory_SkillKeeper.Model;

namespace MapleStory_SkillKeeper
{
    [AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel
    {
        public const string MapleStoryNotFound = "MapleStory not found!";

        [JsonIgnore]
        public string Status { get; set; } = MapleStoryNotFound;

        [JsonIgnore]
        public string Slogan { get; set; } = "他山之石可以攻錯";

        public string MapleStoryProcessName { get; set; } = "MapleStory";

        public bool IsEnable { get; set; } = true;

        [JsonIgnore]
        public ObservableCollection<Skill> Skills { get; set; } = new();

        public int Delay { get; set; } = 1000;
    }
}