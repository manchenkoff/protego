using System;
using System.ComponentModel; //логика привязки данных

namespace Protego.Logic {

    [Serializable]
    public class Extension : INotifyPropertyChanged {
        //уведомление об изменениях значений переменных
        #region Properties Event Logic
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPopertyChanged(string propertyName) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        //закрытые строки названия расширения и его описания
        private string _extName, _extDesc;

        /// <summary>
        /// Наименование расширения (.txt)
        /// </summary>
        public string Name {
            get { return _extName; }
            set {
                if (_extName != value)
                    _extName = value; OnPopertyChanged("Name");
            }
        }

        /// <summary>
        /// Описание расширения
        /// </summary>
        public string Description {
            get { return _extDesc; }
            set {
                if (_extDesc != value)
                    _extDesc = value; OnPopertyChanged("Description");
            }
        }

        /// <summary>
        /// Базовый конструктор с заданными параметрами
        /// </summary>
        /// <param name="extension">Строка расширения (.txt)</param>
        /// <param name="description">Описание файла или расширения</param>
        public Extension(string extension, string description) {
            Name = extension;
            Description = description;
        }
    }

}
