using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace PostOfficeClient.Tables_Providers
{
    public class PostOfficesProvider : TablesProvider
    {
        public PostOfficesProvider(string sqlCommand = "SELECT * FROM PostOffices") : base(sqlCommand)
        {
            foreach (DataRow row in Table.Rows)
            {
                row[3] = row[3].ToString().Replace("@", "\n");
            }
        }

        public IEnumerable GetIndexesOfOffices()
        {
            List<int> indexesList = new List<int>();

            if (Table != null)
            {
                foreach (DataRow curOffice in Table.Rows)
                {
                    indexesList.Add((int)curOffice["Index"]);
                }
            }

            return indexesList;
        }
    }
}
