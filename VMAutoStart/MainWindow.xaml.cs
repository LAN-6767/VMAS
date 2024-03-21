
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace VMAutoStart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<ListitenC> listitenC;
        private Config configG;
        private OpenFolderDialog openFolderDialog;
        private UserConfig userConfig;
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            if (!VMC.CheckVmrun())
            {
                MessageBox.Show("'vmrun'不是内部或外部命令，也不是可运行的程序或批处理文件\n 将VMware目录添加进环境变量");
                this.Close();
            }
            else
            {
                cmdtip.Text = Runcmdresult.Runcmd("vmrun");
            }
            listitenC = new List<ListitenC>();
            openFolderDialog = new OpenFolderDialog();
            configG = new Config();
            userConfig = new UserConfig(this);
            list1.ItemsSource = listitenC;
            ConfigC.Init();
            Loadvminfo();
        }
        private void Button_Click_ref(object sender, RoutedEventArgs e)
        {
            Loadvminfo();
        }

        private void Button_Click_add(object sender, RoutedEventArgs e)
        {

            openFolderDialog.ShowDialog();
            string[] vmxFiles;
            string directoryPath = openFolderDialog.FolderName;
            try
            {
                vmxFiles = Directory.GetFiles(directoryPath, "*.vmx");
            }
            catch (Exception)
            {
                vmxFiles = null;
            }
            if (vmxFiles != null && vmxFiles.Any())
            {
                List<ConfigBase> listt = configG.Vmxs.ToList();
                foreach (string file in vmxFiles)
                {
                    ConfigBase configBase = new ConfigBase()
                    {
                        Isrun = false,
                        IsAutorun = false,
                    };
                    configBase.Path = file;
                    listt.Add(configBase);
                }
                configG.Vmxs = listt.ToArray();
                ConfigC.Writefile(configG);
                Loadvminfo();
            }
            else
            {
                MessageBox.Show("不存在 *.vmx 的文件");
            }
        }
        private void Loadvminfo()
        {
            listitenC.Clear();
            Config isrunconfig = new Config() { Vmxs = VMC.GetConfig() };
            Config flieconfig = ConfigC.Readfile();
            configG = isrunconfig + flieconfig;
            ConfigC.Writefile(configG);
            for (int i = 0; i < configG.Vmxs.Length; i++)
            {
                listitenC.Add(configG.Vmxs[i]);
            }
            this.Dispatcher.Invoke(new Action(() =>
            {
                list1.Items.Refresh();
            }));
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            String commparam = menuItem.CommandParameter as String;
            ListitenC listitenC1 = list1.SelectedItem as ListitenC;
            switch (commparam)
            {
                case "open":
                    OpenDirectoryInExplorer(Path.GetDirectoryName(listitenC1.Path));
                    break;
                case "delete":
                    DelectVmPath(listitenC1);
                    break;
                default:
                    cmdtip.Text = VMC.VmrunType3(menuItem.Header.ToString(), listitenC1.Path, Loadvminfo);
                    break;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            userConfig.ChangeWindowSizeInfo(this);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            userConfig.SaveWindowSize(this);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            userConfig.UpdataWindowSize(this);
        }
        static void OpenDirectoryInExplorer(string directoryPath)
        {
            try
            {
                Process.Start("explorer.exe", directoryPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening directory: " + ex.Message);
            }
        }
        private void DelectVmPath(ListitenC PathUi)
        {
            listitenC.Remove(PathUi);
            list1.Items.Refresh();
            configG.Vmxs = ListitenC.ToConfigBaseArray(listitenC);
            ConfigC.Writefile(configG);
        }
    }
}