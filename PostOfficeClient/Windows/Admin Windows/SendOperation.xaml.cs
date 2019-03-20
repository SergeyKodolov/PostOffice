using PostOfficeClient.Tables_Providers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace PostOfficeClient.Windows.Admin_Windows
{
    /// <summary>
    /// Логика взаимодействия для SendOperation.xaml
    /// </summary>
    public partial class SendOperation : Window
    {
        Window previousForm;
        int index = 0;
        MailingsProvider mailings;
        TrackingProvider tracking;

        public SendOperation(object o, int _index)
        {
            InitializeComponent();
            mailings = new MailingsProvider();
            tracking = new TrackingProvider();
            index = _index;

            mailingsList.ItemsSource = mailings.GetMailingsInOfficeList(tracking, index);

            mailingsInOffice.ItemsSource = mailings.GetMailingsInOffice(tracking, index);

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
        
        private void SendChecked_Click(object sender, RoutedEventArgs e)
        {
            if (mailingsList.Items.Count == 0)
            {
                MessageBox.Show("Нечего отправлять!");
                return;
            }

            if (mailingsList.SelectedItems.Count == 0)
            {
                MessageBox.Show("Ничего не выбрано!");
                return;
            }

            foreach (string trackingNumber in mailingsList.SelectedItems)
            {
                DataRow newTracking = tracking.Table.NewRow();

                newTracking["TrackNumber"] = trackingNumber;
                newTracking["Index"] = index;
                newTracking["IsSent"] = true;

                tracking.Table.Rows.Add(newTracking);
            }

            if (tracking.Insert() != null)
                return;

            Close();
            MessageBox.Show("РПО успешно отправлены!");
        }

        private void MailingsList_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            selectedMailings.Items.Clear();
            foreach (DataRow mailing in mailings.Table.Rows)
            {
                if (mailingsList.SelectedItems.Contains(mailing["TrackingNumber"].ToString()))
                {
                    selectedMailings.Items.Add(mailing.ItemArray);
                }
            }
        }
    }
}
