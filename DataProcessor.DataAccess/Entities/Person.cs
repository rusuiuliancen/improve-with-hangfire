using System;

namespace DataProcessor.DataAccess.Entities
{
    public class Person
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public bool EmailSent { get; set; }
        public string? ProcessException { get; set; }
    }
}
