using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Banks.Accounts;

namespace Banks.Clients
{
    public struct ClientInfo
    {
        public List<Account> Accounts;
        public bool IsDoubtful;
        private string _address;
        private string _phoneNumber;
        private string _firstName;
        private string _lastName;
        private string _passportNumber;

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (PhoneNumberIsInvalid(value))
                    throw new ArgumentException("Phone number is invalid (format: +7 (xxx) xxx-xxxx)");
                _phoneNumber = value;
            }
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("First name mustn't be null or empty");
                _firstName = value;
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Last name mustn't be null or empty");
                _lastName = value;
            }
        }

        public string PassportNumber
        {
            get => _passportNumber;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _passportNumber = null;
                    return;
                }

                if (PassportNumberIsInvalid(value))
                    throw new ArgumentException("Passport number is invalid");
                _passportNumber = value;

                IsDoubtful = _passportNumber == null && _address == null;
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                if (string.IsNullOrEmpty(value))
                    _address = value;
                _address = value;

                IsDoubtful = _passportNumber == null && _address == null;
            }
        }

        public static bool PassportNumberIsInvalid(string num) => !Regex.IsMatch(num, "^\\d{6}$");
        public static bool PhoneNumberIsInvalid(string num) => !Regex.IsMatch(num, @"^\+7 \(\d{3}\) \d{3}-\d{4}$");
    }
}