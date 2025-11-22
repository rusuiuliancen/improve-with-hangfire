using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using DataProcessor.DataAccess.Entities;
using DataProcessor.DataAccess;
using Microsoft.Extensions.Logging;
using DataProcessor.Business.Contracts;
using DataProcessor.Business.Dtos;

namespace DataProcessor.Business.Services
{
    public class PersonProcessorService : IPersonProcessorService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<PersonProcessorService> _logger;

        public PersonProcessorService(AppDbContext dbContext, ILogger<PersonProcessorService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public CsvProcessResult ProcessCsv(Stream csvStream)
        {
            int totalProcessed = 0;
            int totalInserted = 0;
            int totalInvalid = 0;
            using var reader = new StreamReader(csvStream);
            string? line;
            bool isHeader = true;
            while ((line = reader.ReadLine()) != null)
            {
                if (isHeader)
                {
                    isHeader = false;
                    continue;
                }
                totalProcessed++;
                var fields = line.Split(',');
                if (fields.Length != 7)
                {
                    totalInvalid++;
                    _logger.LogWarning($"Invalid row: {line}");
                    continue;
                }
                var person = ParsePerson(fields);
                var validationErrors = ValidatePerson(person);
                if (validationErrors.Count > 0)
                {
                    totalInvalid++;
                    _logger.LogWarning($"Invalid row: {line} | Errors: {string.Join(", ", validationErrors)}");
                    continue;
                }
                _dbContext.Persons.Add(person);
                _dbContext.SaveChanges();
                totalInserted++;
            }
            return new CsvProcessResult
            {
                TotalProcessed = totalProcessed,
                TotalInserted = totalInserted,
                TotalInvalid = totalInvalid
            };
        }

        private Person ParsePerson(string[] fields)
        {
            return new Person
            {
                FirstName = fields[0].Trim(),
                LastName = fields[1].Trim(),
                Email = fields[2].Trim(),
                Phone = fields[3].Trim(),
                Address = fields[4].Trim(),
                Country = fields[5].Trim(),
                DateOfBirth = DateTime.TryParseExact(fields[6].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dob) ? dob : DateTime.MinValue
            };
        }

        private List<string> ValidatePerson(Person person)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(person.FirstName) || person.FirstName.Length < 2)
                errors.Add("FirstName invalid");
            if (string.IsNullOrWhiteSpace(person.LastName) || person.LastName.Length < 2)
                errors.Add("LastName invalid");
            if (string.IsNullOrWhiteSpace(person.Email) || !Regex.IsMatch(person.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                errors.Add("Email invalid");
            if (string.IsNullOrWhiteSpace(person.Phone) || !Regex.IsMatch(person.Phone, @"^[0-9\-\+\(\)\s]{7,}$"))
                errors.Add("Phone invalid");
            if (string.IsNullOrWhiteSpace(person.Address))
                errors.Add("Address invalid");
            if (string.IsNullOrWhiteSpace(person.Country))
                errors.Add("Country invalid");
            if (person.DateOfBirth == DateTime.MinValue || person.DateOfBirth > DateTime.Today)
                errors.Add("DateOfBirth invalid");
            return errors;
        }
    }
}
