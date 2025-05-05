using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{


    public class PasswordHasher : IPasswordHasher
    {
    public string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // Compute the hash of the password
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            // Convert byte array to a string
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
    public bool VerifyHashedPassword(string providedPassword, string hashedPassword)
    {
        string providedPasswordHash = HashPassword(providedPassword);

        return providedPasswordHash.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase);
    }
}


}
