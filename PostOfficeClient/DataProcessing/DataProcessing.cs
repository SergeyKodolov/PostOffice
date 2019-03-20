using suggestionscsharp;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace PostOfficeClient
{
    public enum typeOfField { Address, EMail, Name };
    
    /// <summary>
    /// Класс, обрабатывающий вводимые пользователем данные
    /// </summary>
    static class DataProcessing
    {
        static string token;
        static string url;

        public static SuggestClient api { get; set; }

        static DataProcessing()
        {
            var data = File.ReadAllLines(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()))) + @"\data.txt", Encoding.Default);
            token = data[3];
            url = "https://suggestions.dadata.ru/suggestions/api/4_1/rs";
            api = new SuggestClient(token, url);            
        }

        /// <summary>
        /// Приводит имена к нормализованному виду
        /// </summary>
        /// <param name="name">Имя для нормализации</param>
        /// <returns>Нормализованное имя</returns>
        public static string NameNormalize(string name)
        {
            char[] nameToChar = name.ToCharArray();
            nameToChar[0] = char.ToUpper(nameToChar[0]);
            for (int i = 1; i < nameToChar.Length; i++)
            {
                nameToChar[i] = char.ToLower(nameToChar[i]);                
            }
            return string.Concat(nameToChar);
        }       

        /// <summary>
        /// Проверка email, ФИО или адреса
        /// </summary>
        /// <param name="data">Передаваемые для проверки данные</param>
        /// <param name="field">Категория передаваемых данных</param>
        /// <returns></returns>
        public static bool isDataCorrect(string data, typeOfField field)
        {
            string pattern = null;
            if (field == typeOfField.EMail)
            {
                pattern = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";
            }
            else if (field == typeOfField.Address)
            {
                pattern = @"^\d{6} [\w\W]+";
            }
            else if (field == typeOfField.Name)
            {
                pattern = @"^[а-я]+";
            }

            if (pattern != null)
                if (Regex.IsMatch(data, pattern, RegexOptions.IgnoreCase))
                    return true;

            return false;            
        }

        /// <summary>
        /// Формирует список РПО в отделении
        /// </summary>
        /// <param name="trackNumber">Трек РПО</param>
        /// <param name="trackings">Таблица отслеживания</param>
        /// <param name="index">Индекс отделения</param>
        /// <returns></returns>
        public static bool isMailingInOffice(string trackNumber, DataRowCollection trackings, int index)
        {
            int countOfTracking = 0;

            foreach (DataRow track in trackings)
            {
                if ((trackNumber == track["TrackNumber"].ToString()
                    && ((int)track["Index"] == index)))
                {
                    countOfTracking++;
                }
            }

            if (countOfTracking % 2 == 0)
            {
                return false;
            }
            else
            {
                return true;
            }           
        }
    }
}
