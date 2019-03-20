using IronBarCode;
using PostOfficeClient.Tables_Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using Xceed.Words.NET;

namespace PostOfficeClient.Windows.Admin_Windows
{
    /// <summary>
    /// Логика взаимодействия для Papers.xaml
    /// </summary>
    public partial class Papers : Window
    {
        Window previousForm;
        int index = 0;
        List<DataRow> mailingsThere = new List<DataRow>();
        MailingsProvider mailings;
        TrackingProvider tracking;
        UsersProvider users;
        PostOfficesProvider postOffices;

        public Papers(object o, int _index)
        {
            InitializeComponent();
            mailings = new MailingsProvider();
            tracking = new TrackingProvider();
            users = new UsersProvider();
            postOffices = new PostOfficesProvider();
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

        private void CreateNotice_Click(object sender, RoutedEventArgs e)
        {
            if (mailingsList.SelectedIndex == -1)
            {
                MessageBox.Show("Ничего не выбрано!");
                return;
            }

            string trackNumber = mailingsList.SelectedValue.ToString();
            string path = "Извещение " + trackNumber + ".docx";

            DocX doc = DocX.Create(path, DocumentTypes.Document);           
            
            BarcodeWriter.CreateBarcode(
                trackNumber,
                BarcodeWriterEncoding.Code128, 100, 50)                
                .SaveAsPng(trackNumber + ".png"); 

            var picture = doc.AddImage(trackNumber + ".png").CreatePicture();
            var label = doc.AddImage("pochta.png").CreatePicture(50, 100);
            var p = doc.InsertParagraph().AppendPicture(label);
            p.Append("\t\t\t\t\t\t\t\t").AppendPicture(picture);
            
            File.Delete(trackNumber + ".png");

            doc.InsertParagraph("Извещение №" + trackNumber).FontSize(18).SpacingAfter(25).Bold().Alignment = Alignment.center;

            p = doc.InsertParagraph("Кому: ").FontSize(14);
            p.Append(firstName.Text + " " + lastName.Text).FontSize(14).Bold();
            p = doc.InsertParagraph("E-Mail: ").FontSize(14);
            p.Append(email.Text).FontSize(14).Bold();
            p = doc.InsertParagraph("Адрес: ").FontSize(14);
            p.Append(address.Text).FontSize(14).Bold();
            p = doc.InsertParagraph("Вид и категория: ").FontSize(14);
            p.Append(mailingType.Text).FontSize(14).Bold();
            doc.InsertParagraph("Внимание! Срок хранения 30 дней!").SpacingAfter(15).Bold().SpacingBefore(15);

            doc.InsertParagraph("Выдача производится по адресу: " + officeAddress.Text);
            doc.InsertParagraph("Телефон для справок: " + officePhone.Text);
            doc.InsertParagraph("Расписание работы отделения: ");
            doc.InsertParagraph(workingHours.Text);

            doc.Save();           

            Process.Start(path);
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

                    foreach (DataRow office in postOffices.Table.Rows)
                    {
                        if ((int)office["Index"] == index)
                        {
                            officeAddress.Text = office["Address"].ToString();
                            officePhone.Text = office["Phone"].ToString();
                            workingHours.Clear();
                            workingHours.AppendText(office["WorkingHours"].ToString());
                            break;
                        }
                    }

                    break;
                }
            }
        }       
    }
}
