using System.Data;
using System.Data.SqlClient;

namespace PostOfficeClient.Tables_Providers
{
    public class TrackingProvider : TablesProvider
    {
        public TrackingProvider(string sqlCommand = "SELECT * FROM Tracking") : base(sqlCommand) { }

        public DataTable GetTrackingForSelectedDate(string selectedDate)
        {
            DataTable table = new DataTable();
            connection.Open();
            string command = "SELECT * FROM Tracking WHERE " +
                "CONVERT(DATE, RegistrationDateTime) = '" +
                selectedDate + "'";
            SqlCommand getTrackingCommand = new SqlCommand(command, connection);
            var trackingReader = getTrackingCommand.ExecuteReader();
            table.Load(trackingReader);
            connection.Close();
            return table;
        }

        public DataTable GetTrackingForSelectedDate(int index)
        {
            DataTable table = new DataTable();
            connection.Open();
            string command = "SELECT * FROM Tracking WHERE " +
                "[Index] = " + index;
            SqlCommand getTrackingCommand = new SqlCommand(command, connection);
            var trackingReader = getTrackingCommand.ExecuteReader();
            table.Load(trackingReader);
            connection.Close();
            return table;
        }


        public DataTable GetTrackingForSelectedDate(string selectedDate, int index)
        {
            DataTable table = new DataTable();
            connection.Open();
            string command = "SELECT * FROM Tracking WHERE " +
                "CONVERT(DATE, RegistrationDateTime) = '" +
                selectedDate + "' AND [Index] = " + index;
            SqlCommand getTrackingCommand = new SqlCommand(command, connection);
            var trackingReader = getTrackingCommand.ExecuteReader();
            table.Load(trackingReader);
            connection.Close();
            return table;
        }
    }
}
