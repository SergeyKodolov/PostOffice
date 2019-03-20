using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PostOfficeClient.Tables_Providers
{
    public class MailingsProvider : TablesProvider
    {
        public List<List<string>> TypesOfMailing { get; private set; }

        public MailingsProvider(string sqlCommand = "SELECT * FROM Mailings") : base (sqlCommand)
        {
            TypesOfMailing = new List<List<string>>();

            TypesOfMailing.Add(new List<string>()
                    {
                        "Письмо",
                        "Заказное письмо",
                        "Заказное письмо 1 класса",
                        "Ценное письмо",
                        "Ценное письмо 1 класса",
                        "Экспресс-письмо EMS"
                    });
            TypesOfMailing.Add(new List<string>()
                    {
                        "Бандероль",
                        "Заказная бандероль",
                        "Заказная бандероль 1 класса",
                        "Ценная бандероль",
                        "Ценная бандероль 1 класса"
                    });
            TypesOfMailing.Add(new List<string>()
                    {
                        "Мелкий пакет",
                        "Заказной мелкий пакет",
                        "Посылка",
                        "Ценная посылка",
                        "Экспресс-посылка EMS"
                    });
        }

        public int GetNumOfMailingsInThisMonth()
        {
            connection.Open();
            string command = "SELECT COUNT(DISTINCT(TrackNumber)) FROM Tracking WHERE " +
                "YEAR(RegistrationDateTime) = YEAR(GETDATE()) AND " +
                "MONTH(RegistrationDateTime) = MONTH(GETDATE())";
            SqlCommand numOfMailingsCommand = new SqlCommand(command, connection);
            int numOfMailingsInMonth = (int)numOfMailingsCommand.ExecuteScalar();
            connection.Close();
            return numOfMailingsInMonth+1;
        }

        public IEnumerable GetMailingsInOfficeList(TrackingProvider tracking, int index)
        {
            List<string> mailingsList = new List<string>();

            if (Table != null)
            {
                foreach (DataRow mailing in Table.Rows)
                {
                    if ((bool)mailing["Delivered"])
                        continue;

                    if (DataProcessing.isMailingInOffice(mailing["TrackingNumber"].ToString(), tracking.Table.Rows, index))
                    {
                        mailingsList.Add(mailing["TrackingNumber"].ToString());
                    }
                }
            }

            return mailingsList;
        }

        public IEnumerable GetMailingsNotInOfficeList(TrackingProvider tracking, int index)
        {
            List<string> mailingsList = new List<string>();

            if (Table != null)
            {
                foreach (DataRow mailing in Table.Rows)
                {
                    if ((bool)mailing["Delivered"])
                        continue;

                    if (!DataProcessing.isMailingInOffice(mailing["TrackingNumber"].ToString(), tracking.Table.Rows, index))
                    {
                        mailingsList.Add(mailing["TrackingNumber"].ToString());
                    }
                }
            }

            return mailingsList;
        }

        public List<DataRow> GetMailingsInOffice(TrackingProvider tracking, int index)
        {
            List<DataRow> mailings = new List<DataRow>();

            if (Table != null)
            {
                foreach (DataRow mailing in Table.Rows)
                {
                    if ((bool)mailing["Delivered"])
                        continue;

                    if (DataProcessing.isMailingInOffice(mailing["TrackingNumber"].ToString(), tracking.Table.Rows, index))
                    {
                        mailings.Add(mailing);
                    }
                }
            }

            return mailings;
        }

        public string GenerateTrackingNumber(int index)
        {
            DateTime beginDate = new DateTime(2000, 1, 1);
            DateTime nowDate = DateTime.Now;
            int numOfMonth = (((nowDate.Year - beginDate.Year) * 12) + nowDate.Month - beginDate.Month) % 99;
            string countOfMailings = string.Format("{0,0:D5}", GetNumOfMailingsInThisMonth());
            string withoutCheck = string.Concat(index, numOfMonth, countOfMailings);
            int check = GenerateCheck(withoutCheck);
            return string.Concat(withoutCheck, check);
        }

        private int GenerateCheck(string withoutCheck)
        {
            char[] arr = withoutCheck.ToCharArray();

            int evenSum = 0, oddSum = 0;

            for (int i = 0; i < 13; i++)
            {
                if (i % 2 == 0)
                {
                    evenSum += (int)char.GetNumericValue(arr[i]);
                }
                else
                {
                    oddSum += (int)char.GetNumericValue(arr[i]);
                }
            }
            return 9 - ((oddSum + evenSum * 3) % 9);
        }
    }
}
