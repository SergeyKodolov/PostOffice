using ImapX;
using ImapX.Enums;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using MailAddress = System.Net.Mail.MailAddress;

namespace PostOfficeService
{
    public partial class Service : ServiceBase
    {
        ImapClient client;
        Timer timer = new Timer();
        string[] data;
        SqlConnection sqlConnection;

        public Service()
        {
            InitializeComponent();            
        }

        protected override void OnStart(string[] args)
        {
            //System.Threading.Thread.Sleep(10000);
            WriteToFile("Стартуем! " + DateTime.Now);

            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 15000;
            timer.Enabled = true;

            data = File.ReadAllLines(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)))) + @"\data.txt", Encoding.Default);
            sqlConnection = new SqlConnection(data[2]);            

            client = new ImapClient("imap.yandex.ru", 993, true);
            if (client.Connect())
            {
                WriteToFile("Подключение успешно! " + DateTime.Now);
                
                if (client.Login(data[0], data[1]))
                {
                    WriteToFile("Аутентификация успешна! " + DateTime.Now);
                }
                else
                {
                    WriteToFile("Аутентификация провалилась! " + DateTime.Now);
                }
            }
            else
            {
                WriteToFile("Ошибка подключения! " + DateTime.Now);
            }
        }

        protected override void OnStop()
        {
            if (client != null)
                client.Disconnect();
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            Message[] letters = client.Folders["INBOX"].Search("ALL", MessageFetchMode.Minimal, 1);

            foreach (Message letter in letters)
            {
                if (letter.Seen == false)
                {                    
                    var subject = letter.Subject.Substring(0, 14);
                    var fromEmail = letter.From.Address.ToString();
                    
                    letter.Seen = true;

                    WriteToFile("Проверка трэка! " + DateTime.Now);

                    if (isSubjectCorrect(subject))
                        GenerateAnswer(fromEmail, subject);
                }
            }
        }

        private bool isSubjectCorrect(string subject)
        {
            var correct = false;

            sqlConnection.Open();

            string command = "SELECT COUNT(DISTINCT(TrackNumber)) FROM Tracking WHERE TrackNumber=" + subject;
            SqlCommand getTrackingCommand = new SqlCommand(command, sqlConnection);

            if ((int)getTrackingCommand.ExecuteScalar() == 1)
                correct = true;

            sqlConnection.Close();

            return correct;
        }

        private void GenerateAnswer(string toEmail, string subject)
        {
            string trackNumber = subject;

            SendMail(
             data[0],
             toEmail,
             "Отслеживание по номеру " + trackNumber,
             GetTrackingMessage(trackNumber)
            );            
        }        

        private string GetTrackingMessage(string trackNumber)
        {
            WriteToFile("Генерируем письмо! " + DateTime.Now);

            if (trackNumber != "" && trackNumber.Length == 14)
            {
                var mailingRow = GetMailingRow(trackNumber);
                var trackingRows = GetTrackingRows(trackNumber);
                
                StringBuilder report = new StringBuilder();

                report.AppendLine("Информация о РПО");
                report.AppendLine();
                report.AppendLine("Тип посылки: " + mailingRow.Rows[0]["TypeOfMailing"].ToString());
                report.AppendLine("Адрес получателя: " + mailingRow.Rows[0]["Address"].ToString());
                if (mailingRow.Rows[0]["AddresserEmail"].ToString() != "")
                    report.AppendLine("Email отправителя: " + mailingRow.Rows[0]["AddresserEmail"].ToString());
                if (mailingRow.Rows[0]["AddresseeEmail"].ToString() != "")
                    report.AppendLine("Email получателя: " + mailingRow.Rows[0]["AddresseeEmail"].ToString());
                report.AppendLine();

                foreach (DataRow track in trackingRows.Rows)
                {
                    report.Append(
                        track["RegistrationDateTime"].ToString() + ": "
                        );
                    if (!(bool)track["IsSent"])
                    {
                        report.Append("Прибыло в отделение ");
                    }
                    else
                    {
                        report.Append("Покинуло отделение  ");
                    }
                    report.AppendLine(
                        track["Index"].ToString()
                        );
                }

                return report.ToString();
            }

            throw new Exception("Что-то пошло не так");
        }

        private DataTable GetMailingRow(string trackNumber)
        {
            DataTable table = new DataTable();

            sqlConnection.Open();

            string sqlCommand = "SELECT * FROM Mailings WHERE TrackingNumber=" + trackNumber;
            SqlCommand command = new SqlCommand(sqlCommand, sqlConnection);
            var reader = command.ExecuteReader();
            table.Load(reader);

            sqlConnection.Close();

            return table;
        }

        private DataTable GetTrackingRows(string trackNumber)
        {
            DataTable table = new DataTable();

            sqlConnection.Open();

            string sqlCommand = "SELECT * FROM Tracking WHERE TrackNumber = " + trackNumber + " ORDER BY RegistrationDateTime ASC";
            SqlCommand command = new SqlCommand(sqlCommand, sqlConnection);
            var reader = command.ExecuteReader();
            table.Load(reader);

            sqlConnection.Close();

            return table;
        }

        public MailMessage SendMail(string from, string to, string subject, string text)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(from);
                mail.To.Add(new MailAddress(to));
                mail.Subject = subject;
                mail.Body = text;               

                SmtpClient client = new SmtpClient();
                client.Host = "smtp.yandex.ru";
                client.Port = 25;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(data[0], data[1]);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mail);

                WriteToFile("Письмо отправлено! " + DateTime.Now);

                return mail;
            }
            catch (Exception e)
            {
                WriteToFile("Ошибка! " + DateTime.Now + " " + e.Message);
                throw new Exception("Mail.Send: " + e.Message);
            }
        }
                
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}