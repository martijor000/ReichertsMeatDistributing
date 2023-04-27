using Microsoft.AspNetCore.Components;
using System.Security.Cryptography;
using System.Text;

namespace ReichertsMeatDistributing.Client.Pages
{
    partial class Login
    {
        private string _username = "RFBfWlZcVVxdVQ==";
        private string _password = "ZFhcWA0HDw94";

        private string key = "0123456789abcdef";

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

            if(Convert.ToBase64String(EncryptStringToBytes_XOR(Username.ToLower(), key)) == _username && Convert.ToBase64String(EncryptStringToBytes_XOR(Password, key)) == _password)
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

        private static byte[] EncryptStringToBytes_XOR(string plainText, string key)
        {
            var keyBytes = Encoding.ASCII.GetBytes(key);
            var plainBytes = Encoding.UTF8.GetBytes(plainText);

            var encryptedBytes = new byte[plainBytes.Length];

            for (int i = 0; i < plainBytes.Length; i++)
            {
                encryptedBytes[i] = (byte)(plainBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return encryptedBytes;
        }


    }
}
