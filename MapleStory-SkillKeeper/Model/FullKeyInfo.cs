namespace MapleStory_SkillKeeper.Model
{
    [AddINotifyPropertyChangedInterface]
    public class FullKeyInfo : IFullKeyInfo, ICloneable
    {
        public Keys Key { get; set; }

        public int ScanCode { get; set; }

        public bool IsExtented { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}