using PostOfficeClient.Tables_Providers;
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace PostOfficeClient.Windows.Admin_Windows
{
    /// <summary>
    /// Логика взаимодействия для MailingsReceiving.xaml
    /// </summary>
    public partial class MailingsReceiving : Window
    {
        Window previousForm;
        int index = 0;
        string[] data;
        MailingsProvider mailings;
        TrackingProvider tracking;
        UsersProvider users;

        public MailingsReceiving(object o, int _index)
        {
            InitializeComponent();
            mailings = new MailingsProvider();
            tracking = new TrackingProvider();
            users = new UsersProvider();
            index = _index;
            trackingNumber.Text = mailings.GenerateTrackingNumber(index);

            data = File.ReadAllLines(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)))) + @"\data.txt", Encoding.Default);

            if (o is Admin)
            {
                previousForm = (Admin)o;
            }                    
        }

        protected override void OnClosed(EventArgs e)
        {
            previousForm.Show();
            previousForm.Activate();
            base.OnClosed(e);
        }        
        
        private void Type_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int selectedIndex = type.SelectedIndex;

            if (selectedIndex == -1)
                return;

            category.ItemsSource = mailings.TypesOfMailing[selectedIndex];           
        }

        private void Address_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (address.SelectedItem != null)
            {
                var suggestion = (suggestionscsharp.SuggestAddressResponse.Suggestions)address.SelectedItem;

                if (suggestion != null)
                    if (suggestion.data.postal_code != null)
                    {
                        var sourceAddress = address.SelectedItem;
                        suggestion.value = suggestion.data.postal_code + " " + sourceAddress;
                        address.SelectedItem = suggestion;
                    }
            }
        }

        private void Address_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var response = DataProcessing.Api.QueryAddress(address.Text);
                address.ItemsSource = response.suggestions;
                address.IsDropDownOpen = true;
            }
        }

        private void AddMailing_Click(object sender, RoutedEventArgs e)
        {
            if (type.SelectedIndex == -1 || category.SelectedIndex == -1)
            {
                MessageBox.Show("Укажите вид и категорию РПО!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }                

            if (addresseeEmail.Text != "")
            {
                if (!DataProcessing.isDataCorrect(addresseeEmail.Text, typeOfField.EMail))
                {
                    MessageBox.Show("Некорректный E-Mail", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            if (addresserEmail.Text != "")
            {
                if (!DataProcessing.isDataCorrect(addresserEmail.Text, typeOfField.EMail))
                {
                    MessageBox.Show("Некорректный E-Mail", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            

            if (address.Text != "")
            {
                if (!DataProcessing.isDataCorrect(address.Text, typeOfField.Address))
                {
                    MessageBox.Show("Адрес некорректен или неточен!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Введите адрес!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CreateMailing();
            CreateTracking();
            if (mailings.Insert() != null || tracking.Insert() != null)
                return;

            if (addresseeEmail.Text != "" || addresserEmail.Text != "")
            {
                SendTrackToUsers();
            }

            Close();
            MessageBox.Show("РПО успешно добавлено!");
        }

        private void SendTrackToUsers()
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(data[0]);

                if (addresseeEmail.Text != "")
                    if (users.IsUserRegistered(addresseeEmail.Text))
                    {
                        mail.To.Add(new MailAddress(addresseeEmail.Text));
                    }

                if (addresserEmail.Text != "")
                    if (users.IsUserRegistered(addresserEmail.Text))
                    {
                        mail.To.Add(new MailAddress(addresserEmail.Text));
                    }

                mail.Subject = "Трек-номер отслеживания РПО";
                mail.Body = trackingNumber.Text.Replace(" ", "");

                SmtpClient client = new SmtpClient();
                client.Host = "smtp.yandex.ru";
                client.Port = 25;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(data[0], data[1]);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mail);
            }
            catch (Exception ex)
            {
                throw new Exception("Mail.Send: " + ex.Message);
            }
        }

        private void CreateMailing()
        {
            DataRow newMailing = mailings.Table.NewRow();

            newMailing["TrackingNumber"] = trackingNumber.Text.Replace(" ", "");
            newMailing["TypeOfMailing"] = category.Text;
            newMailing["DestinationIndex"] = int.Parse(address.Text.Substring(0, 6));
            newMailing["Address"] = address.Text;
            
            if (addresseeEmail.Text != "")
                newMailing["AddresseeEmail"] = addresseeEmail.Text;

            if (addresserEmail.Text != "")
                newMailing["AddresserEmail"] = addresserEmail.Text;

            mailings.Table.Rows.Add(newMailing);
        }

        private void CreateTracking()
        {
            DataRow newTracking = tracking.Table.NewRow();

            newTracking["TrackNumber"] = trackingNumber.Text.Replace(" ", "");
            newTracking["Index"] = index; 

            tracking.Table.Rows.Add(newTracking);
        }
    }
}
