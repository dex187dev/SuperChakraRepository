using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SuperChakra.system
{
    public class ChakraSecurity
    {
        // DATENQUELLEN

        private readonly HashSet<string> _allowedViews = new HashSet<string>(StringComparer.OrdinalIgnoreCase) 
        { 
            "meta", "physics", "binary", "entity"
        };

        // FELDER

        // KONSTRUKTOR

        public ChakraSecurity() 
        { 
            
        }

        // METHODEN

        public bool IsViewAllowed(string viewName)
        {
            if (string.IsNullOrWhiteSpace(viewName)) return false;

            bool isAllowed = _allowedViews.Contains(viewName.Trim());
            if (!isAllowed)
            {
                Debug.WriteLine($"=== ⚠️ SECURITY WARNUNG: Unautorisierter Ansichten-Zugriff blockiert: '{viewName}' ===");
            }
            return isAllowed;
        }

        public string SanitizeInput(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            return input.Replace("'", "''").Trim();
        }

        public string CalculateScriptHash(string sqlScript)
        {
            if (string.IsNullOrEmpty(sqlScript)) return string.Empty;

            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(sqlScript);
                byte[] hashBytes = sha512.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {                    
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

    }
}

