using System;

namespace GalaxiBackend
{
    public class Client
    {
        public string Phonenumber { get; set; }
        public string Name { get; set; }
        public Faculty Faculty { get; set; }
        public string Email { get; set; }
        public int Year { get; set; }
    }
    public class Faculty
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    public class CheckInHistory
    {
        public DateTime CheckIn { get;set; }
        public DateTime? CheckOut { get; set; }
        public float TotalPrice { get; set; }
        public bool IsCheckedOut { get => CheckOut != null; }
    }
    public class Payment
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public int Stock { get; set; }
    }
    public class Purchase
    {
        public Payment BasePayment { get; set; }
        public float PaidPrice { get; set; }
        public int Amount { get; set; }
    }
}
