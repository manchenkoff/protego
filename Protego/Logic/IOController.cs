using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Protego.Logic {

    public class IOController : INotifyPropertyChanged {

        #region Properties Event Logic
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPopertyChanged(string propertyName) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Properties
        //коллекции устройств и файлов
        private List<DriveInfo> _drives;
        private ObservableCollection<FileInfo> _files;

        /// <summary>
        /// Список внешних устройств
        /// </summary>
        public List<DriveInfo> Drives {
            get { return _drives; }
            set {
                if (_drives != value)
                    _drives = value; OnPopertyChanged("Drives");
            }
        }

        /// <summary>
        /// Список опасных файлов
        /// </summary>
        public ObservableCollection<FileInfo> DangerFiles {
            get { return _files; }
            set {
                if (_files != value)
                    _files = value; OnPopertyChanged("DangerFiles");
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Инициализация контроллера устройств и файлов
        /// </summary>
        public IOController() {
            //инициализируем коллекции носителей и файлов
            Drives = new List<DriveInfo>();
            DangerFiles = new ObservableCollection<FileInfo>();

            //получаем список внешних устройств
            Drives = GetUSBDrives();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Метод для получения списка устройств с типом Removable
        /// </summary>
        /// <returns>Коллекция DriveInfo</returns>
        public List<DriveInfo> GetUSBDrives() {
            //создаем временную коллекцию
            List<DriveInfo> temp = new List<DriveInfo>();

            //циклом для всех устройств отбираем с типом Removable
            foreach (var drive in DriveInfo.GetDrives()) {
                //и добавляем во временную коллекцию
                if (drive.DriveType == DriveType.Removable) temp.Add(drive);
            }
            //возвращаем коллекцию как результат
            return temp;
        }

        /// <summary>
        /// Метод проверки устройства на наличие вредоносных файлов
        /// </summary>
        /// <param name="drive">Устройство для проверки</param>
        /// <param name="extensions">Коллекция опасных расширений</param>
        public void ScanDevice(DriveInfo drive, ObservableCollection<Extension> extensions) {
            //если устройство передано
            if (drive != null) {
                //инициализируем коллекции файлов и опасных файлов
                List<FileInfo> files = new List<FileInfo>();
                ObservableCollection<FileInfo> dangers = new ObservableCollection<FileInfo>();

                //получаем список всех файлов во всех директориях на устройстве
                files.AddRange(GetFiles(drive.RootDirectory));

                //сбрасываем атрибуты файлов и папок
                ResetAttributes(drive.RootDirectory);

                //проверяем расширение каждого файла
                foreach (var file in files) {
                    foreach (var extension in extensions) {
                        if (file.Extension == extension.Name) {
                            //если найдено из списка опасных, добавляем в коллекцию
                            dangers.Add(file);
                        }
                    }
                }
                //возвращаем коллекцию опасных файлов
                DangerFiles = dangers;
            }
        }

        /// <summary>
        /// Метод рекурсивного получения всех файлов в папке
        /// </summary>
        /// <param name="dir">Директория для поиска</param>
        /// <returns></returns>
        private List<FileInfo> GetFiles(DirectoryInfo dir) {
            //коллекция для списка файлов
            List<FileInfo> files = new List<FileInfo>();
            //получаем список файлов в текущей директории
            files.AddRange(dir.GetFiles());
            //если есть подпапки
            foreach (var subDir in dir.GetDirectories()) {
                //запускаем рекурсивную функцию
                files.AddRange(GetFiles(subDir));
            }
            //возвращаем коллекцию всех файлов
            return files;
        }

        /// <summary>
        /// Удаление всех файлов с расширением *.lnk
        /// </summary>
        public void DeleteLinks() {
            //коллекция для ярлыков
            List<FileInfo> links = new List<FileInfo>();

            //перебираем коллекцию файлов на соответствие ярлыку
            foreach (var file in DangerFiles) {
                if (file.Extension == ".lnk") {
                    //добавляем во временную коллекцию
                    links.Add(file);
                    //удаляем из основной
                    DangerFiles.Remove(file);
                }
            }

            //удаляем все совпадения с устройства
            links.ForEach(x => x.Delete());
        }

        /// <summary>
        /// Удаление выбранных файлов
        /// </summary>
        /// <param name="files">Коллекция файлов</param>
        public void DeleteSelected(List<FileInfo> files) {
            //перебираем коллекцию
            foreach (var file in files) {
                //удаляем файл из коллекции
                DangerFiles.Remove(file);
                //удаляем файл с устройства
                file.Delete();
            }
        }

        /// <summary>
        /// Удалить все опасные файлы
        /// </summary>
        public void DeleteAll() {
            //удаляем все файлы в коллекции
            foreach (var file in DangerFiles) {
                file.Delete();
            }
            //очищаем коллекцию файлов
            DangerFiles.Clear();
        }


        /// <summary>
        /// Сброс атрибутов файлов и папок на стандартные
        /// </summary>
        /// <param name="dir">Директория для обработки</param>
        private void ResetAttributes(DirectoryInfo dir) {
            try {
                //если директория скрыта
                if (dir.Name != "System Volume Information" &&
                    (dir.Attributes & FileAttributes.Hidden) == (FileAttributes.Hidden)) {
                    //ставим стандартный набор атрибутов
                    File.SetAttributes(dir.FullName, FileAttributes.Directory | FileAttributes.Normal);
                }
                //для каждого файла в директории
                foreach (var file in dir.GetFiles()) {
                    //ставим нормальные атрибуты
                    file.Attributes = FileAttributes.Normal;
                }
                //выполняем рекусрию для всех подпапок
                foreach (var subDir in dir.GetDirectories()) {
                    ResetAttributes(subDir);
                }

            } catch (Exception e) {
                //в случае ошибки выводим сообщение
                System.Windows.Forms.MessageBox.Show(string.Format("Error - {0}", e.Message));
            }
        }
        #endregion
    }
}
