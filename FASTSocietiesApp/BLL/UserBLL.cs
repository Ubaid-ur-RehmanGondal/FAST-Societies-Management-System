using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using FASTSocietiesApp.DAL;
using FASTSocietiesApp.Helpers;
using FASTSocietiesApp.Models;

namespace FASTSocietiesApp.BLL
{
    /// <summary>
    /// Business Logic Layer for user authentication and registration.
    /// Owns: input validation, password hashing, and role rules.
    /// Calls UserDAL only; never opens a SqlConnection itself.
    /// </summary>
    public class UserBLL
    {
        private readonly UserDAL _userDal;

        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public UserBLL()
        {
            _userDal = new UserDAL();
        }

        /// <summary>
        /// Authenticates a user by email and plain-text password.
        /// Hashes the password before delegating to the DAL.
        /// Returns the matching UserModel or null if credentials are invalid.
        /// </summary>
        public UserModel? AuthenticateUser(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new AppException("Email is required.");
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required.");

            try
            {
                string normalizedEmail = email.Trim();
                string passwordHash = HashPassword(password);
                return _userDal.GetUserByEmailAndPassword(normalizedEmail, passwordHash);
            }
            catch (AppException ex)
            {
                throw new AppException("Authentication failed: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Registers a new student account. Role is forced to 'Student'.
        /// Validates inputs, checks email uniqueness, and persists via DAL.
        /// Returns the generated UserId.
        /// </summary>
        public int RegisterStudent(string fullName, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new AppException("Full name is required.");
            if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email))
                throw new AppException("A valid email address is required.");
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                throw new AppException("Password must be at least 6 characters long.");
            if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
                throw new AppException("Password and confirmation do not match.");

            try
            {
                string normalizedEmail = email.Trim();

                if (_userDal.EmailExists(normalizedEmail))
                {
                    throw new AppException("An account with this email already exists.");
                }

                UserModel newUser = new UserModel
                {
                    FullName = fullName.Trim(),
                    Email = normalizedEmail,
                    PasswordHash = HashPassword(password),
                    Role = "Student",
                    IsActive = true
                };

                int newId = _userDal.InsertUser(newUser);
                if (newId <= 0)
                {
                    throw new AppException("Registration did not return a valid user id.");
                }
                return newId;
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("UserBLL.RegisterStudent failed.", ex);
                throw new AppException("Registration failed due to an unexpected error.", ex);
            }
        }

        /// <summary>
        /// Maps the database role string to the strongly typed UserRole enum
        /// used by SessionManager. Throws AppException for unknown values.
        /// </summary>
        public UserRole ParseRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                throw new AppException("Role is empty.");

            return role switch
            {
                "Student" => UserRole.Student,
                "SocietyHead" => UserRole.SocietyHead,
                "Admin" => UserRole.Admin,
                _ => throw new AppException("Unknown role: " + role)
            };
        }

        /// <summary>
        /// Produces a SHA-256 hex hash of the given plain-text password.
        /// Centralised so registration and login always agree on format.
        /// </summary>
        public static string HashPassword(string password)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = SHA256.HashData(bytes);
            StringBuilder sb = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
