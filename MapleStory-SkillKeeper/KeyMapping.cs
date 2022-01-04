using System.Linq;
using MapleStory_SkillKeeper.Model;

namespace MapleStory_SkillKeeper
{
    internal class KeyMapping
    {
        public static Dictionary<string, FullKeyInfo>? KeyMappingDictionary { get; set; }

        public static void InitDictionary()
        {
            SharedFunctions.LoadJsonFile("KeyMapping.json", out Dictionary<string, FullKeyInfo>? keyMapping);
            if (keyMapping is not null)
                KeyMappingDictionary = keyMapping.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
        }
    }
}