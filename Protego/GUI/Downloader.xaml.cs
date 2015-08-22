using System.Windows;
using System.Net;
using System;
using System.IO;
using System.Diagnostics;
using Protego.Logic;

namespace Protego.GUI {
    /// <summary>
    /// Interaction logic for Downloader.xaml
    /// </summary>
    public partial class Downloader : Window {

        public Downloader() {
            InitializeComponent();
            Init();
        }

        void Init() {

            string filename = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads",
                "Protego Setup.exe"
                );

            using (WebClient web = new WebClient()) {
                web.DownloadFileAsync(
                    new Uri(Updater.SetupURL),
                    filename
                    );

                web.DownloadProgressChanged += (s, e) => {
                    downloadProgressBar.Value = e.ProgressPercentage;
                };

                web.DownloadFileCompleted += (s, e) => {
                    Process setup = new Process {
                        StartInfo = new ProcessStartInfo(filename)
                    };
                    setup.Start();
                    App.Current.Shutdown();
                };
            }
        }

        private void cancelClick(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
