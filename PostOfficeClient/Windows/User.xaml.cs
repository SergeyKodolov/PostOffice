using PostOfficeClient.Tables_Providers;
using System;
using System.Text;
using System.Windows;

namespace PostOfficeClient
{
    /// <summary>
    /// Логика взаимодействия для User.xaml
    /// </summary>
    public partial class User : Window
    {
        Window previousForm;
        MailingsProvider mailings;
        TrackingProvider tracking;

        public User(object o)
        {
            InitializeComponent();
            mailings = new MailingsProvider();
            tracking = new TrackingProvider();

            if (o is MainWindow)
            {
                previousForm = (MainWindow)o;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            previousForm.Show();
            previousForm.Activate();
            base.OnClosed(e);
        }

        private void Tracking_Click(object sender, RoutedEventArgs e)
        {
            string trackNumber = trackingNumber.Text.Replace(" ", "");
            trackNumber = trackNumber.Replace("_", "");

            if (trackNumber != "" && trackNumber.Length == 14)
            {
                var mailingRow = mailings.Table.Select("TrackingNumber=" + trackNumber);
                var trackingRows = tracking.Table.Select("TrackNumber=" + trackNumber, "RegistrationDateTime ASC");

                if (mailingRow.Length == 0)
                {
                    MessageBox.Show("Данный трек-номер не зарегистрирован");
                    return;
                }

                StringBuilder report = new StringBuilder();

                report.AppendLine("Информация о РПО");
                report.AppendLine();
                report.AppendLine("Тип посылки: " + mailingRow[0]["TypeOfMailing"].ToString());
                report.AppendLine("Адрес получателя: " + mailingRow[0]["Address"].ToString());
                if (mailingRow[0]["AddresserEmail"].ToString() != "")
                    report.AppendLine("Email отправителя: " + mailingRow[0]["AddresserEmail"].ToString());
                if (mailingRow[0]["AddresseeEmail"].ToString() != "")
                    report.AppendLine("Email получателя: " + mailingRow[0]["AddresseeEmail"].ToString());
                report.AppendLine();

                foreach (var track in trackingRows)
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

                MessageBox.Show(report.ToString());
            }
        }
    }
}
