using PostOfficeClient.Tables_Providers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace PostOfficeClient.Windows.Admin_Windows
{
    /// <summary>
    /// Логика взаимодействия для MailingsDelivery.xaml
    /// </summary>
    public partial class MailingsDelivery : Window
    {
        Window previousForm;
        int index = 0;
        List<DataRow> mailingsThere = new List<DataRow>();
        MailingsProvider mailings;
        TrackingProvider tracking;
        UsersProvider users;

        public MailingsDelivery(object o, int _index)
        {
            InitializeComponent();
            mailings = new MailingsProvider();
            tracking = new TrackingProvider();
            users = new UsersProvider();
            index = _index;

            mailingsList.ItemsSource = mailings.GetMailingsInOfficeList(tracking, index);

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

        private void DeliveryMailing_Click(object sender, RoutedEventArgs e)
        {
            if (mailingsList.SelectedIndex == -1)
            {
                MessageBox.Show("Ничего не выбрано!");
                return;
            }

            string trackingNumber = mailingsList.SelectedValue.ToString();

        DataRow mailing = mailings.Table.Select("TrackingNumber="+trackingNumber)[0];
            mailing["Delivered"] = true;

            DataRow newTracking = tracking.Table.NewRow();

            newTracking["TrackNumber"] = trackingNumber;
            newTracking["Index"] = index;
            newTracking["IsSent"] = true;

            tracking.Table.Rows.Add(newTracking);        

            if (tracking.Insert() != null || mailings.Insert() != null)
                return;           

            Close();
            MessageBox.Show("РПО успешно выдано!");
        }

        private void MailingsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            foreach (DataRow mailing in mailings.Table.Rows)
            {
                if (mailingsList.SelectedValue.ToString() == mailing["TrackingNumber"].ToString())
                {
                    mailingType.Text = mailing["TypeOfMailing"].ToString();
                    address.Text = mailing["Address"].ToString();
                    if (mailing["AddresseeEmail"].ToString() != "")
                    {
                        email.IsEnabled = true;
                        firstName.IsEnabled = true;
                        lastName.IsEnabled = true;

                        email.Text = mailing["AddresseeEmail"].ToString();

                        foreach (DataRow user in users.Table.Rows)
                        {
                            if (user["EMail"].ToString() == email.Text)
                            {
                                firstName.Text = user["FirstName"].ToString();
                                lastName.Text = user["LastName"].ToString();
                                break;
                            }
                        }
                    }
                    else
                    {
                        email.IsEnabled = false;
                        firstName.IsEnabled = false;
                        lastName.IsEnabled = false;
                    }
                    break;
                }
            }
        }
    }
}
