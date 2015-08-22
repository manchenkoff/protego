using System.Net; //работа с сетью

namespace Protego.Logic {
    public static class Updater {
        //текущая версия программы
        public static string CurrentVersion = "1.0";
        //версия для обновления
        private static string _newVersion { get; set; }
        //проверка доступности обновлений
        public static bool UpdatesAvailable {
            get {
                //создаем веб-клиент для загрузки строки из файла
                using (WebClient web = new WebClient()) {
                    _newVersion = web.DownloadString(
                        //адрес для загрузки строки
                        "http://apps.manchenkoff.me/protego/downloads/protego.ver"
                        );
                    //если новая версия не равна строке текущей - возваращаем true
                    return (CurrentVersion != _newVersion) ? true : false;
                }
            }
        }
        //путь для скачивания установщика
        public static string SetupURL {
            get {
                return "http://apps.manchenkoff.me/protego/downloads/protego.exe";
            }
        }
    }
}
