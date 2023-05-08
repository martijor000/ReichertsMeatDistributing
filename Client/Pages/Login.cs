using Microsoft.AspNetCore.Components;
using System;
using System.Security.Cryptography;
using System.Text;

namespace ReichertsMeatDistributing.Client.Pages
{
    partial class Login
    {
        private string _username = "O1Kz713QmFQaK8xLIivOXn4Mp0e/3/C4ENt2QabsaPc=";
        private string _password = "BFRWcZjVHw7j22niLhzUaR3rsSm/XeMy5/si5qnHLtk=";

        private string key = "PrG9Q5lH63YpmOVmzMLgmceREYsvQFecxPfaK1Bht/k=";

        private string Username { get; set; }
        private string Password { get; set; }
        private string ErrorMessage { get; set; } // Add a property to store error message

        private void HandleLoginFormSubmit()
        {
            if (string.IsNullOrEmpty(Username))
            {
                ErrorMessage = "Please enter your username";
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Please enter your password";
                return;
            }

            string hashedUsername = HashStringWithSalt(Username.ToLower(), key);
            string hashedPassword = HashStringWithSalt(Password, key);

            if (hashedUsername == _username && hashedPassword == _password)
            {
                AuthStateProvider.MarkUserAsAuthenticated("admin");
                NavigationManager.NavigateTo("/admin/deals");
            }
            else
            {
                ErrorMessage = "Invalid username or password";
                return;
            }
        }

        private static string HashStringWithSalt(string input, string salt)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

            byte[] hashedBytes;
            using (var sha256 = new SHA256Managed())
            {
                byte[] saltedInput = new byte[inputBytes.Length + saltBytes.Length];
                Buffer.BlockCopy(inputBytes, 0, saltedInput, 0, inputBytes.Length);
                Buffer.BlockCopy(saltBytes, 0, saltedInput, inputBytes.Length, saltBytes.Length);

                hashedBytes = sha256.ComputeHash(saltedInput);
            }

            return Convert.ToBase64String(hashedBytes);
        }
    }
}
