using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxiBackend
{
    static public class Galaxi
    {
        static public Client GetClient(string Phonenumber)
        {
            Client[] res = DatabaseManager.GetClients(new ClientQuery { Filter = ClientQueryFilter.Phonenumber, Value = Phonenumber });
            if (res == null)
                return null;
            return res[0];
        }
        static public bool ClientHasCheckedIn(Client client)
        {
            var res = DatabaseManager.GetLastCheckInHistory(client.Phonenumber);
            if(res == null)
            {
                return false;
            }
            return !res.IsCheckedOut;
        }
        static public CheckInHistory[] GetCheckInHistory(Client client)
        {
            return DatabaseManager.GetCheckinHistory(client.Phonenumber);
        }
        static public CheckInHistory GetLastCheckin(Client client)
        {
            return DatabaseManager.GetLastCheckInHistory(client.Phonenumber);
        }
        static public void CheckInClient(Client client)
        {
            CheckInHistory lastCheckIn = GetLastCheckin(client);
            if (lastCheckIn != null && !lastCheckIn.IsCheckedOut)
            {
                lastCheckIn.CheckOut = lastCheckIn.CheckIn;
                DatabaseManager.InsertIntoCheckInHistory(client.Phonenumber, lastCheckIn);
            }
            DatabaseManager.InsertIntoCheckInHistory(client.Phonenumber, new CheckInHistory { CheckIn = DateTime.Now, CheckOut = null });
        }
        static public void CheckOutClient(Client client)
        {
            CheckInHistory lastCheckIn = GetLastCheckin(client);
            if(lastCheckIn == null || lastCheckIn.IsCheckedOut)
            {
                throw new Exception("Client hasn't checked in to checkout");
            }
            lastCheckIn.CheckOut = DateTime.Now;
            DatabaseManager.InsertIntoCheckInHistory(client.Phonenumber, lastCheckIn);
        }
        static public Payment[] GetAllPayments()
        {
            return DatabaseManager.GetPayments();
        }
    }
}
