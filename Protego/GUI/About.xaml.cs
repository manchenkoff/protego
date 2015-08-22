using System.Windows;
using Protego.Logic;

namespace Protego.GUI {
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window {

        public About() {
            InitializeComponent();
            Init();
        }

        void Init() {
            appVersion.Content = string.Format("Версия {0}", Updater.CurrentVersion);
            appDescription.Text = Properties.Resources.history;
        }
    }
}
