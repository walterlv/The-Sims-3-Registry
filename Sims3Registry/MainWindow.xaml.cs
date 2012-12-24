using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using m3i;
using m3i.SimsRegistry;

namespace Sims3Registry
{
    public enum InstallState
    {
        Install,
        Fix,
        Modify,
        Modified,
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private InstallState TobeState = InstallState.Install;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GameComboBox.SelectedIndex = 0;
        }

        #region CdKey
        bool isTypingCdkey = true;
        private string CdKey
        {
            get
            {
                return CdkeyText1.Text + CdkeyText2.Text + CdkeyText3.Text + CdkeyText4.Text + CdkeyText5.Text;
            }
            set
            {
                if (value.Length != 20) return;
                isTypingCdkey = false;
                CdkeyText1.Text = value.Substring(0, 4);
                CdkeyText2.Text = value.Substring(4, 4);
                CdkeyText3.Text = value.Substring(8, 4);
                CdkeyText4.Text = value.Substring(12, 4);
                CdkeyText5.Text = value.Substring(16, 4);
                isTypingCdkey = true;
            }
        }
        private void CdkeyText1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            if (CdkeyText1.Text.Length >= 4) { if (isTypingCdkey) { CdkeyText2.Focus(); CdkeyText2.SelectAll(); } }
        }
        private void CdkeyText2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            if (CdkeyText2.Text.Length >= 4) { if (isTypingCdkey) { CdkeyText3.Focus(); CdkeyText3.SelectAll(); } }
        }
        private void CdkeyText3_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            if (CdkeyText3.Text.Length >= 4) { if (isTypingCdkey) { CdkeyText4.Focus(); CdkeyText4.SelectAll(); } }
        }
        private void CdkeyText4_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            if (CdkeyText4.Text.Length >= 4) { if (isTypingCdkey) { CdkeyText5.Focus(); CdkeyText5.SelectAll(); } }
        }
        private void CdkeyText5_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            if (CdkeyText5.Text.Length >= 4) return;
        }
        #endregion

        #region InstallDir
        public string InstallDir
        {
            get { return InstallDirText.Text; }
            set { InstallDirText.Text = value; }
        }
        #endregion

        #region Country
        public string Country
        {
            get
            {
                return IndexToCountry(CountryBox.SelectedIndex);
            }
            set
            {
                CountryBox.SelectedIndex = CountryToIndex(value);
            }
        }
        private string IndexToCountry(int index)
        {
            switch (index)
            {
                case 0:
                    return "CN";
                default:
                    return "CN";
            }
        }
        private int CountryToIndex(string country)
        {
            if (country.Equals("CN")) return 0;
            else return -1;
        }
        #endregion

        #region LocaleId
        public int LocaleId
        {
            get
            {
                return IndexToLocaleId(LanguageBox.SelectedIndex);
            }
            set
            {
                LanguageBox.SelectedIndex = LocaleIdToIndex(value);
            }
        }
        private int IndexToLocaleId(int index)
        {
            switch (index)
            {
                case 0:
                    return 0x00;
                case 1:
                    return 0x01;
                case 2:
                    return 0x02;
                case 3:
                    return 0x02;
                case 4:
                    return 0x0C;
                case 5:
                    return 0x0D;
                case 6:
                    return 0x16;
                default:
                    return 0x02;
            }
        }
        private int LocaleIdToIndex(int id)
        {
            switch (id)
            {
                case 0x00:
                    return 0;
                case 0x01:
                    return 1;
                case 0x02:
                    return 2;
                case 0x0C:
                    return 4;
                case 0x0D:
                    return 5;
                case 0x16:
                    return 6;
                default:
                    return -1;
            }
        }
        #endregion

        #region SKU
        public int Sku
        {
            get
            {
                int result;
                if (Int32.TryParse(SkuBox.Text, out result)) return result;
                else return 2;
            }
            set
            {
                SkuBox.Text = value.ToString();
            }
        }
        #endregion

        #region 改变资料片
        /// <summary>
        /// <para>指定当前的状态是由下拉框改变的还是由文本框改变的</para>
        /// <para>false代表由下拉框改变, true表示由文本框改变.</para>
        /// </summary>
        bool ByText = false;
        private void GameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GameComboBox.SelectedIndex < 0) return;
            SimsRegistryInfo info = new SimsRegistryInfo(GameComboBox.SelectedIndex);
            if (info.IsInstalled)
            {
                if (File.Exists(info.ExePath))
                {
                    try
                    {
                        BitmapImage icon = GetBitmapImage(info.InstallDir + @"\Game\Bin\Sims3" + info.Pack.PackName + ".ico");
                        IconImage.Source = icon;
                    }
                    catch
                    {
                        IconImage.Source = null;
                    }
                    ChangeState(InstallState.Modify);
                }
                else
                {
                    IconImage.Source = null;
                    ChangeState(InstallState.Fix);
                }
                InstallDir = info.InstallDir;
                DirInfoText.Text = null;
                Country = info.Country;
                if (info.LocaleId != null) LocaleId = (int)info.LocaleId;
                if (info.Sku != null) Sku = (int)info.Sku;
                CdKey = info.EaErgc;
                InstallButton.IsEnabled = true;
            }
            else
            {
                IconImage.Source = null;
                ChangeState(InstallState.Install);
                if (ByText) { InstallDirText_TextChanged(null, null); ByText = false; }
                else { InstallButton.IsEnabled = false; InstallDir = null; }
            }
            TitleText.Text = (GameComboBox.SelectedItem as ComboBoxItem).Content.ToString();
        }

        private void InstallDirButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folder = new System.Windows.Forms.FolderBrowserDialog();
            folder.ShowNewFolderButton = false;
            folder.SelectedPath = InstallDir;
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                InstallDir = folder.SelectedPath;
            }
            InstallDirText.Focus();
            InstallDirText.SelectAll();
        }

        private Brush InfoBrush = new SolidColorBrush(Colors.SteelBlue);
        private Brush WarnBrush = new SolidColorBrush(Colors.Red);
        private void InstallDirText_TextChanged(object sender, TextChangedEventArgs e)
        {
            GamePack pack = GamePack.GetGamePackByIndex(GameComboBox.SelectedIndex);
            if (GameComboBox.SelectedIndex < 0) return;
            if (InstallDir.Length <= 0) { DirInfoText.Text = null; InstallButton.IsEnabled = false; return; }
            if (File.Exists(InstallDir + @"\Game\Bin\Ts3" + pack.PackName + ".exe"))
            {
                switch (TobeState)
                {
                    case InstallState.Install:
                        DirInfoText.Foreground = InfoBrush;
                        DirInfoText.Text = "发现这正好是《" + (GameComboBox.SelectedItem as ComboBoxItem).Content.ToString() + "》的游戏目录" + Environment.NewLine + "点击“安装”可以将它添加到注册表。";
                        InstallButton.IsEnabled = true;
                        break;
                    default:
                        DirInfoText.Text = null;
                        InstallButton.IsEnabled = true;
                        break;
                }
            }
            else if (File.Exists(InstallDir + @"\Game\Bin\Ts3" + pack.PackName + ".ex_"))
            {
                switch (TobeState)
                {
                    case InstallState.Install:
                        DirInfoText.Foreground = InfoBrush;
                        DirInfoText.Text = "发现这正好是《" + (GameComboBox.SelectedItem as ComboBoxItem).Content.ToString() + "》数位版的安装目录" + Environment.NewLine + "点击“安装”可以立即开始安装。";
                        InstallButton.IsEnabled = true;
                        break;
                    default:
                        DirInfoText.Text = null;
                        InstallButton.IsEnabled = true;
                        break;
                }
            }
            else
            {
                string dir = InstallDir + @"\Game\Bin";
                if (Directory.Exists(dir))
                {
                    for (int i = 0; i < GamePack.Count; i++)
                    {
                        pack = GamePack.GetGamePackByIndex(i);
                        if (File.Exists(dir + @"\TS3" + pack.PackName + ".exe"))
                        {
                            ByText = true;
                            GameComboBox.SelectedIndex = i;
                            return;
                        }
                        else if (File.Exists(dir + @"\TS3" + pack.PackName + ".ex_"))
                        {
                            ByText = true;
                            GameComboBox.SelectedIndex = i;
                            return;
                        }
                    }
                    DirInfoText.Foreground = WarnBrush;
                    DirInfoText.Text = "这个路径不是任何资料片或物品包的安装路径！";
                    InstallButton.IsEnabled = false;
                }
                DirInfoText.Foreground = WarnBrush;
                DirInfoText.Text = "这个路径不是任何资料片或物品包的安装路径！";
                InstallButton.IsEnabled = false;
            }
        }
        #endregion

        #region 显示隐藏高级设置
        private Storyboard _expandStory;
        public Storyboard ExpandStory
        {
            get
            {
                if (_expandStory == null) _expandStory = this.FindResource("ExpandStory") as Storyboard;
                return _expandStory;
            }
        }
        private Storyboard _collapseStory;
        public Storyboard CollapseStory
        {
            get
            {
                if (_collapseStory == null) _collapseStory = this.FindResource("CollapseStory") as Storyboard;
                return _collapseStory;
            }
        }
        private void AdvanceExpander_Expanded(object sender, RoutedEventArgs e)
        {
            ChangeState(TobeState);
            AdvanceExpander.Header = "显示较少选项";
            BeginStoryboard(ExpandStory);
        }

        private void AdvanceExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            ChangeState(TobeState);
            AdvanceExpander.Header = "显示更多选项";
            BeginStoryboard(CollapseStory);
        }
        #endregion

        #region 关于信息
        private Storyboard _aboutShowStory;
        public Storyboard AboutShowStory
        {
            get
            {
                if (_aboutShowStory == null) _aboutShowStory = this.FindResource("AboutShowStory") as Storyboard;
                return _aboutShowStory;
            }
        }
        private Storyboard _aboutHideStory;
        public Storyboard AboutHideStory
        {
            get
            {
                if (_aboutHideStory == null) _aboutHideStory = this.FindResource("AboutHideStory") as Storyboard;
                return _aboutHideStory;
            }
        }
        private void AboutGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            BeginStoryboard(AboutShowStory);
        }

        private void AboutGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            BeginStoryboard(AboutHideStory);
        }
        #endregion

        #region 安装
        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            string msg = null;
            SimsRegistryInfo info = new SimsRegistryInfo(GameComboBox.SelectedIndex, true);
            info.SetInfo(InstallDir, Country, LocaleId, Sku, CdKey);
            if (TobeState == InstallState.Install)
            {
                try
                {
                    if (Directory.Exists(InstallDir + @"_Installer")) Directory.Delete(InstallDir + @"_Installer", true);
                    if (!File.Exists(InstallDir + @"\Game\Bin\TS3" + info.Pack.PackName + @".exe")) File.Move(InstallDir + @"\Game\Bin\TS3" + info.Pack.PackName + @".ex_", InstallDir + @"\Game\Bin\TS3" + info.Pack.PackName + @".exe");
                    if (!File.Exists(InstallDir + @"\Game\Bin\Sims3Launcher.exe")) File.Move(InstallDir + @"\Game\Bin\Sims3Launcher.ex_", InstallDir + @"\Game\Bin\Sims3Launcher.exe");
                }
                catch { msg = Environment.NewLine + "但是，安装过程中所需要的文件可能缺失。" + Environment.NewLine + "如果您在打开游戏启动器时提示“在开始期间有一个错误”，请使用本程序删除掉这个注册表信息。"; }
                msg = "《" + GetNameByIndex(info.Pack.Index) + "》已经安装完成！" + msg;
                UpdateState();
                MessageBox.Show(msg, "安装完成", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (TobeState == InstallState.Modify)
            {
                MessageBox.Show("已经保存对《" + GetNameByIndex(info.Pack.Index) + "》所作的更改", "保存修改", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            SimsRegistryInfo info = new SimsRegistryInfo(GameComboBox.SelectedIndex, true);
            MessageBoxResult result = MessageBox.Show("确定要删除《" + GetNameByIndex(info.Pack.Index) + "》的所有注册表信息吗？", "删除注册表信息", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK) info.Delete();
            else return;
            UpdateState();
        }
        private void UpdateState()
        {
            int index = GameComboBox.SelectedIndex;
            GameComboBox.SelectedIndex = -1;
            GameComboBox.SelectedIndex = index;
        }
        #endregion

        #region 通用方法
        public string GetNameByIndex(int index)
        {
            return (GameComboBox.Items[index] as ComboBoxItem).Content.ToString();
        }
        private void ChangeState(InstallState state)
        {
            switch (state)
            {
                case InstallState.Install:
                    StateText.Text = "未安装";
                    InstallButton.Content = "安装";
                    InstallButton.Visibility = System.Windows.Visibility.Visible;
                    DeleteButton.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case InstallState.Fix:
                    StateText.Text = "有错误";
                    InstallButton.Content = "修复";
                    InstallButton.Visibility = System.Windows.Visibility.Visible;
                    DeleteButton.Visibility = System.Windows.Visibility.Visible;
                    break;
                case InstallState.Modify:
                    StateText.Text = "已安装";
                    InstallButton.Content = "保存修改";
                    if (AdvanceExpander.IsExpanded) InstallButton.Visibility = System.Windows.Visibility.Visible;
                    else InstallButton.Visibility = System.Windows.Visibility.Collapsed;
                    DeleteButton.Visibility = System.Windows.Visibility.Visible;
                    break;
                case InstallState.Modified:
                    StateText.Text = "已修改";
                    InstallButton.Content = "保存修改";
                    InstallButton.Visibility = System.Windows.Visibility.Visible;
                    DeleteButton.Visibility = System.Windows.Visibility.Visible;
                    break;
                default:
                    break;
            }
            TobeState = state;
        }
        public static BitmapImage GetBitmapImage(string imageFile)
        {
            // 读取图片源文件到byte[]中
            BinaryReader binReader = new BinaryReader(File.Open(imageFile, FileMode.Open, FileAccess.Read));
            FileInfo fileInfo = new FileInfo(imageFile);
            byte[] bytes = binReader.ReadBytes((int)fileInfo.Length);
            binReader.Close();
            // 将图片字节赋值给Bitmap Image
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = new MemoryStream(bytes);
            bitmap.EndInit();
            return bitmap;
        }
        #endregion
    }
}
