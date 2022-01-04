using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace MapleStory_SkillKeeper.Model
{
    [AddINotifyPropertyChangedInterface]
    public class Skill
    {
        public Skill(Bitmap image, FullKeyInfo keyInfo, int keepOverThen, int delay, string name)
        {
            Image = image;
            KeyInfo = keyInfo;
            Delay = delay;
            KeepOverThen = keepOverThen;
            Name = name;
            Inited = true;
        }

        public Bitmap Image { get; set; }
        public string Name { get; set; }

        private bool Inited { get; }

        [OnChangedMethod(nameof(OnKeyInfoChanged))]
        public FullKeyInfo KeyInfo { get; set; }

        [OnChangedMethod(nameof(OnKeepOverThenChanged))]
        public int KeepOverThen { get; set; }

        [OnChangedMethod(nameof(OnDelayChanged))]
        public int Delay { get; set; }

        private void OnKeyInfoChanged(FullKeyInfo oldValue, FullKeyInfo newValue)
        {
            if (Inited == false)
                return;
            Debug.WriteLine("Name Changed:" + oldValue + " => " + newValue);
            string oldFilename = $"Image\\[{oldValue.Key},{KeepOverThen},{Delay}]{Name}.bmp";
            try
            {
                if (File.Exists(oldFilename))
                    File.Move(oldFilename, Filename, true);
            }
            catch (Exception)
            {
            }
        }

        private void OnKeepOverThenChanged(int oldValue, int newValue)
        {
            if (Inited == false)
                return;
            Debug.WriteLine("Name Changed:" + oldValue + " => " + newValue);
            string oldFilename = $"Image\\[{KeyInfo.Key},{oldValue},{Delay}]{Name}.bmp";
            try
            {
                if (File.Exists(oldFilename))
                    File.Move(oldFilename, Filename, true);
            }
            catch (Exception)
            {
            }
        }

        private void OnDelayChanged(int oldValue, int newValue)
        {
            if (Inited == false)
                return;
            Debug.WriteLine("Name Changed:" + oldValue + " => " + newValue);
            string oldFilename = $"Image\\[{KeyInfo.Key},{KeepOverThen},{oldValue}]{Name}.bmp";
            try
            {
                if (File.Exists(oldFilename))
                    File.Move(oldFilename, Filename, true);
            }
            catch (Exception)
            {
            }
        }

        private string Filename { get => $"Image\\[{KeyInfo.Key},{KeepOverThen},{Delay}]{Name}.bmp"; }
    }
}