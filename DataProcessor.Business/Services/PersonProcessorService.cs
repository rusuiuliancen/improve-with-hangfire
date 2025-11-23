using DataProcessor.Business.Contracts;
using DataProcessor.Business.Dtos;
using DataProcessor.Business.Tenant;
using DataProcessor.DataAccess;
using DataProcessor.DataAccess.Entities;
using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DataProcessor.Business.Services
{
    public class PersonProcessorService : IPersonProcessorService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<PersonProcessorService> _logger;
        private readonly IEmailNotificationService _emailSender;
        private readonly ITenantResolver tenantResolver;

        public PersonProcessorService(AppDbContext dbContext, ILogger<PersonProcessorService> logger, IEmailNotificationService emailSender, ITenantResolver tenantResolver)
        {
            _dbContext = dbContext;
            _logger = logger;
            _emailSender = emailSender;
            this.tenantResolver = tenantResolver;
        }

        public PersonProcessResult ProcessCsv(Stream csvStream)
        {
            int successfulInserts = 0;
            int failedInserts = 0;

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

                var validPerson = ExtractValidPerson(line);
                if (validPerson == null)
                {
                    failedInserts++;
                    continue;
                }

                try 
                {
                    _dbContext.Persons.Add(validPerson);
                    _dbContext.SaveChanges();
                    successfulInserts++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error inserting row: {line}");
                    failedInserts++;
                    continue;
                }
            }

            _emailSender.SendNotifications();

            return new PersonProcessResult
            {
                SuccessfulRecords = successfulInserts,
                FailedRecords = failedInserts
            };
        }

        private Person? ExtractValidPerson(string? line)
        {
            var fields = line.Split(',');
            if (fields.Length != 7)
            {
                _logger.LogWarning($"Invalid row: {line}");
                return null;
            }

            var person = ParsePerson(fields);
            var validationErrors = ValidatePerson(person);
            if (validationErrors.Count > 0)
            {
                _logger.LogWarning($"Invalid row: {line} | Errors: {string.Join(", ", validationErrors)}");
                return null;
            }

            return person;
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
                DateOfBirth = DateTime.TryParseExact(fields[6].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dob) ? dob : DateTime.MinValue,
                Age = DateTime.Today.Year - dob.Year - (dob > DateTime.Today.AddYears(- (DateTime.Today.Year - dob.Year)) ? 1 : 0),
                EmailSent = false
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
            if (_dbContext.Persons.Any(p => p.Email == person.Email))
                errors.Add("Email already exists");
            if (_dbContext.Persons.Any(p => p.Phone == person.Phone))
                errors.Add("Phone already exists");

            return errors;
        }
    }
}
