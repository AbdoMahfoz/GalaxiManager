
namespace GalaxiBackend
{
    public class Client
    {
        public string Phonenumber { get; set; }
        public string Name { get; set; }
        public Faculty Faculty { get; set; }
        public string Email { get; set; }
        public int Year { get; set; }
        public bool CheckedIn { get; set; }
    }
    public class Faculty
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
