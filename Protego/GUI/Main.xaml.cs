using System.IO;
using System.Windows;
using System.Collections.Generic;

using Protego.Logic;

namespace Protego.GUI {
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window {

        #region Properties
        //локальные настройки
        private Settings sets { get; set; }
        //контроллер устройств и файлов
        private IOController controller { get; set; }
        //выбранное из списка устройства
        private DriveInfo CurrentDrive { get; set; }
        //сохранены ли настройки
        private bool _isSaved = true;

        //свойство сохранения настроек
        private bool isSaved {
            get { return _isSaved; }
            set {
                if (_isSaved != value) {
                    _isSaved = value;
                    //если не сохранено, включаем кнопку для сохранения
                    buttonSaveSettings.IsEnabled = !value;
                }
            }
        }
        #endregion

        #region Contructors
        public Main() {
            InitializeComponent();
            Init();
            InitGUI();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Инициализация
        /// </summary>
        void Init() {
            //создаем объект настроек
            sets = new Settings();
            //создаем объект контроллера
            controller = new IOController();
        }

        /// <summary>
        /// Инициализация графичекого интерфейса
        /// </summary>
        void InitGUI() {
            //назначаем контекст для привязки данных
            gridSettings.DataContext = sets;
            DataContext = controller;
            //включаем панель выбора устройств
            showDevicesSection(null, null);
        }
        #endregion

        #region Window actions
        private void helpClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            //выводим окно о программе
            new About().ShowDialog();
        }

        private void FormClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            //если настройки не сохранены - выводим сообщение
            if (!isSaved) {
                switch (System.Windows.Forms.MessageBox.Show(
                    "Измененные настройки не будут сохранены, продолжить?",
                    "Предупреждение",
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Warning
                    )) {
                    //если нажата продолжить
                    case System.Windows.Forms.DialogResult.Yes:
                        e.Cancel = false;
                        break;
                    //если нажата отмена
                    case System.Windows.Forms.DialogResult.No:
                        e.Cancel = true;
                        break;

                }
            }
        }

        private void showDevicesSection(object sender, RoutedEventArgs e) {
            //меняем видимость панелей
            gridCleaner.Visibility = gridSettings.Visibility = Visibility.Hidden;
            gridDevices.Visibility = Visibility.Visible;
        }

        private void showCleanerSection(object sender, RoutedEventArgs e) {
            //если выбранно устройство
            if (CurrentDrive != null) {
                //меняем видимость панелей
                gridDevices.Visibility = gridSettings.Visibility = Visibility.Hidden;
                gridCleaner.Visibility = Visibility.Visible;
                //сканируем устройство
                controller.ScanDevice(CurrentDrive, sets.ExtensionsList);
            } else {
                //иначе выводим сообщение
                System.Windows.Forms.MessageBox.Show("Сначала выберите устройство!");
            }
        }

        private void showSettingsSection(object sender, RoutedEventArgs e) {
            //меняем видимость панелей
            gridCleaner.Visibility = gridDevices.Visibility = Visibility.Hidden;
            gridSettings.Visibility = Visibility.Visible;
        }
        #endregion

        #region Devices section
        private void comboDeviceSelected(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            //при выборе устройства включаем кнопку "перейти к очистке"
            buttonToClean.IsEnabled = true;
            //запоминаем выбранное устройство
            CurrentDrive = (DriveInfo)comboDevices.SelectedItem;
        }

        private void updateDeviceList(object sender, RoutedEventArgs e) {
            //обновляем список устройств
            controller.Drives = controller.GetUSBDrives();
        }

        private void goToCleanSection(object sender, RoutedEventArgs e) {
            //активируем панель очистки
            showCleanerSection(sender, e);
        }

        private void closeWindow(object sender, RoutedEventArgs e) {
            //закрываем окно
            Close();
        }
        #endregion

        #region Cleaner section
        private void deleteLinks(object sender, RoutedEventArgs e) {
            //вызываем метод удаления всех ярлыков
            controller.DeleteLinks();
        }

        private void deleteSelected(object sender, RoutedEventArgs e) {
            //создаем коллекцию для выбранных файлов
            List<FileInfo> selectedFiles = new List<FileInfo>();
            //добавляем в коллекцию все выбранные файлы в списке
            foreach (var item in listDangerFiles.SelectedItems) {
                selectedFiles.Add(item as FileInfo);
            }
            //вызываем метод удаления выбранных файлов
            controller.DeleteSelected(selectedFiles);
        }

        private void deleteAllDangers(object sender, RoutedEventArgs e) {
            //вызываем метод удаления всех опасных файлов
            controller.DeleteAll();
        }
        #endregion

        #region Settings section
        private void Save(object sender, RoutedEventArgs e) {
            //сохраняем настройки
            sets.Save();
            //меняем статус сохранения
            isSaved = true;
        }

        private void addNewExt(object sender, RoutedEventArgs e) {
            //создаем диалог для редактирования расширения
            ExtensionDialog add = new ExtensionDialog();
            //если вернул true
            if (add.ShowDialog() == true) {
                //меняем статус сохранения
                isSaved = false;
                //добавляем новое расширение в настройки
                sets.ExtensionsList.Add((Extension)add.Tag);
            }
        }

        private void editCurrentExt(object sender, RoutedEventArgs e) {
            //если выбранное расширение не null
            if (listExtension.SelectedItem != null) {
                //создаем диалог редактирования расширения
                ExtensionDialog edit = new ExtensionDialog((Extension)listExtension.SelectedItem);
                //если вернул true
                if (edit.ShowDialog() == true) {
                    //меняем статус сохранения
                    isSaved = false;
                }
            }
        }

        private void removeExt(object sender, RoutedEventArgs e) {
            //если выбранный элемент не null
            if (listExtension.SelectedItem != null) {
                //удаляем его из настроек и меняем статус сохранения
                sets.ExtensionsList.Remove(listExtension.SelectedItem as Extension);
                isSaved = false;
            }
        }

        private void checkUpdates(object sender, RoutedEventArgs e) {
            //если доступны обновления
            if (Updater.UpdatesAvailable) {
                updatesTextblock.Text = "Доступна новая версия!";
                //меняем надпись на кнопке и присваиваем обработчик
                buttonCheckUpdates.Content = "Загрузить обновление";
                buttonCheckUpdates.Click += (s, ev) => {
                    //создаем новое окно загрузчика
                    Downloader downloadDialog = new Downloader();
                };
            } else {
                //если обновлений нет - выводим надпись
                updatesTextblock.Text = "Обновлений нет";
            }
        }
        #endregion

    }
}
