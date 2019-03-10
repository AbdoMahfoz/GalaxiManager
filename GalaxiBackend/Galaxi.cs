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
    }
}
