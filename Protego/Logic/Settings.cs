using System;
using System.IO; //работа с фалйами
using System.ComponentModel; //события для привязки данных
using System.Collections.Generic; //работа с коллекциями
using System.Collections.ObjectModel; //работа с коллекциями для привязки

namespace Protego.Logic {

    [Serializable]
    public class Settings : INotifyPropertyChanged {
        //логика уведомлений об изменениях переменных
        #region Properties Event Logic
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPopertyChanged(string propertyName) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Properties
        //коллекция объектов типа Расширение
        private ObservableCollection<Extension> _extensions;
        //путь к файлу настроек
        private string Filename;

        //открытая коллекция расширений
        public ObservableCollection<Extension> ExtensionsList {
            get { return _extensions; }
            set {
                _extensions = value;
                OnPopertyChanged("ExtensionList");
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// базовый конструктор с загрузкой настроек из файла
        /// </summary>
        public Settings() {
            //генерируем путь к файлу и кешируем его
            Filename = Path.Combine(
                Directory.GetCurrentDirectory(),
                "settings.conf"
                );

            //если файл существует - загружаем
            if (File.Exists(Filename)) Load();
            //иначе запускаем инициализацию
            else Init();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Методы для загрузки настроек из файла
        /// </summary>
        public void Load() {
            //десериализуем массив байтов из файла
            ExtensionsList = (Serializer.Deserialize(File.ReadAllBytes(Filename)) as Settings).ExtensionsList;
        }

        /// <summary>
        /// Метод сохранения настроек в файл
        /// </summary>
        public void Save() {
            //пишем сериализованный массив байтов в файл
            File.WriteAllBytes(
                Filename,
                Serializer.Serialize(this)
                );
        }

        /// <summary>
        /// Инициализация настроек
        /// </summary>
        private void Init() {
            //создаем коллекцию расширений по умолчанию
            List<Extension> basicList = new List<Extension> {
                new Extension(".exe", "Исполняемый файл программы"),
                new Extension(".lnk", "Ярлык системы"),
                new Extension(".bat", "Исполняемый скрипт оболочки Windows")
            };

            //инициализируем локальную коллекцию с базовым списком
            ExtensionsList = new ObservableCollection<Extension>(basicList);
            //сохраняем настройки
            Save();
        }
        #endregion
    }
}
