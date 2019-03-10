using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace GalaxiBackend
{
    enum ClientQueryFilter { Phonenumber, name, email, year, facultyid }
    enum FacultyQueryFilter { facultyid, name }
    abstract class Query
    {
        public dynamic Filter;
        public object Value;
    }
    class ClientQuery : Query
    {
        public ClientQuery()
        {
            Filter = new ClientQueryFilter();
        }
    }
    class FacultyQuery : Query
    {
        public FacultyQuery()
        {
            Filter = new FacultyQueryFilter();
        }
    }
    class Reader
    {
        private SqlDataReader reader;
        private SqlConnection connection;
        public Reader(SqlDataReader reader, SqlConnection connection)
        {
            this.reader = reader;
            this.connection = connection;
        }
        public bool Read()
        {
            return reader.Read();
        }
        public void Close()
        {
            reader.Close();
            connection.Close();
        }
        public object this[string key] { get => reader[key]; }
        public object this[int key] { get => reader[key]; }
    }
    static class DatabaseManager
    {
        static readonly string ConnectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;" +
                                                  $"AttachDbFilename={Directory.GetCurrentDirectory()}\\GalaxiDB.mdf;" +
                                                  $"Integrated Security=True";
        static KeyValuePair<SqlCommand, SqlConnection> InitializeQuery(string Query, params KeyValuePair<string, object>[] args)
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();
            SqlCommand command = new SqlCommand()
            {
                Connection = connection,
                CommandText = Query,
                CommandType = CommandType.Text
            };
            foreach (var arg in args)
            {
                command.Parameters.AddWithValue(arg.Key, arg.Value);
            }
            return new KeyValuePair<SqlCommand, SqlConnection>(command, connection);
        }
        static Reader ExecuteQuery(string Query, params KeyValuePair<string, object>[] args)
        {
            var comcon = InitializeQuery(Query, args);
            return new Reader(comcon.Key.ExecuteReader(), comcon.Value);
        }
        static int ExecuteNonQuery(string NonQuery, params KeyValuePair<string, object>[] args)
        {
            var comcon = InitializeQuery(NonQuery, args);
            int res = comcon.Key.ExecuteNonQuery();
            comcon.Value.Close();
            return res;
        }
        static ResultType[] GetData<ResultType, QueryType>(params QueryType[] Queries) where QueryType : Query
        {
            string QueryText = "";
            if (typeof(ResultType) == typeof(Client))
            {
                QueryText = @"SELECT c.Phonenumber, c.Name, c.Email, c.Year, c.facultyid, f.Name, c.checkedIn
                              FROM Clients c, Faculties f
                              WHERE c.facultyid = f.facultyid";
            }
            else
            {
                QueryText = @"SELECT c.facultyid, c.Name
                              FROM Faculties c
                              WHERE 0=0";
            }
            List<KeyValuePair<string, object>> args = new List<KeyValuePair<string, object>>();
            foreach (var query in Queries)
            {
                QueryText += $" AND c.{query.Filter.ToString()} = @{query.Filter.ToString()}";
                args.Add(new KeyValuePair<string, object>(query.Filter.ToString(), query.Value));
            }
            var reader = ExecuteQuery(QueryText, args.ToArray());
            List<object> res = new List<object>();
            while (reader.Read())
            {
                if (typeof(ResultType) == typeof(Client))
                {
                    res.Add(new Client()
                    {
                        Phonenumber = (string)reader[0],
                        Name = (string)reader[1],
                        Email = (string)reader[2],
                        Year = (int)reader[3],
                        Faculty = new Faculty() { ID = (int)reader[4], Name = (string)reader[5] },
                        CheckedIn = (((string)reader[6]) == "T")
                    });
                }
                else
                {
                    res.Add(new Faculty()
                    {
                        ID = (int)reader[0],
                        Name = (string)reader[1]
                    });
                }
            }
            reader.Close();
            if (res.Count == 0)
                return null;
            return (from entry in res
                    select (ResultType)entry).ToArray();
        }
        static public Client[] GetClients(params ClientQuery[] Args)
        {
            return GetData<Client, ClientQuery>(Args);
        }
        static public Faculty[] GetFaculties(params FacultyQuery[] Args)
        {
            return GetData<Faculty, FacultyQuery>(Args);
        }
    }
}
