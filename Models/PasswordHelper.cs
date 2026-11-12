using System.Security.Cryptography;

namespace Itsm.Models
{
    // تشفير كلمات المرور (PBKDF2 مع ملح عشوائي لكل مستخدم)
    public static class PasswordHelper
    {
        private const int Iterations = 600_000;

        public static string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32);
            return $"PBKDF2${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        public static bool Verify(string password, string stored)
        {
            try
            {
                var parts = stored.Split('$');
                if (parts.Length != 4 || parts[0] != "PBKDF2") return false;
                if (!int.TryParse(parts[1], out var iterations)) return false;

                var salt = Convert.FromBase64String(parts[2]);
                var expected = Convert.FromBase64String(parts[3]);
                var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
                return CryptographicOperations.FixedTimeEquals(actual, expected);
            }
            catch
            {
                return false;
            }
        }

        // شروط كلمة المرور الجديدة: 8 أحرف على الأقل، وفيها حرف ورقم. ترجع نص الخطأ أو null.
        public static string? ValidateStrength(string? password)
        {
            if (string.IsNullOrEmpty(password)) return "كلمة المرور مطلوبة.";
            if (password.Length < 8) return "يجب ألا تقل كلمة المرور عن 8 أحرف.";
            if (!password.Any(char.IsLetter)) return "يجب أن تحتوي كلمة المرور على حرف واحد على الأقل.";
            if (!password.Any(char.IsDigit)) return "يجب أن تحتوي كلمة المرور على رقم واحد على الأقل.";
            return null;
        }
    }
}