using Protego.Logic;
using System;
using System.Windows;

namespace Protego.GUI {
    /// <summary>
    /// Interaction logic for ExtensionDialog.xaml
    /// </summary>
    public partial class ExtensionDialog : Window {

        private Extension ext;

        public ExtensionDialog(Extension extension = null) {
            InitializeComponent();
            //кешируем расширение, если было передано
            ext = extension;
            //запускаем инициализацию
            Init();
        }

        void Init() {
            //если расширение было передано
            if (ext != null) {
                //ставим его значения в форму
                extensionTextbox.Text = ext.Name;
                descriptionTextbox.Text = ext.Description;
            }
        }

        private void clickOK(object sender, RoutedEventArgs e) {
            //если расширение было передано
            if (ext != null) {
                //присваиваем ему значения с формы
                ext.Name = extensionTextbox.Text; //имя
                ext.Description = descriptionTextbox.Text; //описание
            } else {
                //иначе создаем новый объект
                ext = new Extension(
                    extensionTextbox.Text,
                    descriptionTextbox.Text
                    );
                //и записываем в Tag формы
                Tag = ext;
            }
            //возвращаем True как результат показа окна
            DialogResult = true;
        }

        private void clickCancel(object sender, RoutedEventArgs e) {
            //возвращаем False как результат показа окна
            DialogResult = false;
        }
    }
}
