using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using MapleStory_SkillKeeper.Plugin;

namespace MapleStory_SkillKeeper
{
    public class SkillKeeperService
    {
        private MainWindowViewModel ViewModel { get; set; }

        public SkillKeeperService(MainWindowViewModel mainWindowViewModel)
        {
            backgroundWorker = new();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            ViewModel = mainWindowViewModel;
            KeyMapping.InitDictionary();
            var imageFilenames = Directory.GetFiles(".\\Image", "*.bmp", SearchOption.TopDirectoryOnly);
            Regex skillFilenameRegex = new(@"\[(.*?),(\d+),(\d+)\](.*).bmp", RegexOptions.Compiled);
            foreach (var imageFilename in imageFilenames)
            {
                var fileName = Path.GetFileName(imageFilename);
                var m = skillFilenameRegex.Match(fileName);
                if (m.Success)
                {
                    string key = m.Groups[1].Value;
                    int overThen = Convert.ToInt32(m.Groups[2].Value);
                    int delay = Convert.ToInt32(m.Groups[3].Value);
                    string name = m.Groups[4].Value;
                    if (KeyMapping.KeyMappingDictionary?.ContainsKey(key) == true)
                    {
                        using Image temp = Image.FromFile(imageFilename);
                        ViewModel.Skills.Add(new(new Bitmap(temp), KeyMapping.KeyMappingDictionary[key], overThen, delay, name));
                    }
                    else
                    {
                        MessageBox.Show($"請檢查Image\\{fileName}的快捷鍵{key}是否設定正確");
                    }
                }
            }
        }

        public void Start()
        {
            if (backgroundWorker.IsBusy == false)
                backgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            BackgroundSimulate backgroundSimulate = new();

            while (true)
            {
                var ps = Process.GetProcessesByName(ViewModel.MapleStoryProcessName);
                SpinWait.SpinUntil(() => ViewModel.IsEnable);
                if (ps.Length == 0)
                {
                    ViewModel.Status = MainWindowViewModel.MapleStoryNotFound;
                    PreciseDelay.Delay(3000);
                    continue;
                }
                ViewModel.Status = $"Found {ps.Length} process";
                foreach (var process in ps)
                {
                    RECT rect = new();
                    if (User32.GetClientRect(process.MainWindowHandle, ref rect) != 0)
                    {
                        int Width = rect.Right - rect.Left;
                        int Height = rect.Bottom - rect.Top;
                        int skillBarHeight = 40;
                        var skillBar = ScreenCapture.Capture(process.MainWindowHandle, Width, skillBarHeight);
                        foreach (var skill in ViewModel.Skills)
                        {
                            if (FindSkill(skill.Image, skill.KeepOverThen))
                            {
                                backgroundSimulate.Hwnd = process.MainWindowHandle;
                                backgroundSimulate.KeyPress(skill.KeyInfo);
                                skill.ConsecutiveTimes++;
                                if (skill.ConsecutiveTimes > 1)
                                    SystemSounds.Beep.Play();
                                PreciseDelay.Delay(skill.Delay);
                                continue;
                            }
                            skill.ConsecutiveTimes = 0;
                        }

                        bool FindSkill(Bitmap bitmap, int percent)
                        {
                            int newSkilIHeight = (int)(bitmap.Height * (100 - percent) / 100d);
                            Bitmap temp = CaptureFromRightDown(bitmap, bitmap.Width, newSkilIHeight);
                            if (skillBar is not null)
                            {
                                var foundedSkillPoint = FindPIC.FindPic(0, 0, Width, skillBarHeight, skillBar, temp, 20).FirstOrDefault();
                                if (foundedSkillPoint.IsEmpty == false)
                                {
                                    return true;
                                }
                            }
                            return false;
                        }
                    }
                }
                PreciseDelay.Delay(ViewModel.Delay);
            }
        }

        private Bitmap CaptureFromRightDown(Bitmap bitmap, int width, int height)
        {
            var newImage = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics.FromImage(newImage).DrawImage(bitmap, 0, 0, new Rectangle(bitmap.Width - width, bitmap.Height - height, width, height), GraphicsUnit.Pixel);
            return newImage;
        }

        private PreciseDelay PreciseDelay { get; } = new();
        private BackgroundWorker backgroundWorker { get; }
    }
}