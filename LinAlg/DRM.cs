using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
namespace LinAlg
{
    // A class which manages access to this software
    class DRM
    {
        public static bool ActivateSoftware(string LicenseKey)
        {
            if (VerifyHash(GetCpuID(), "1219b1c575b61dc6559049b2c863d468e5b8b04811e2d1845e738a410a5b3340"))
            {
                return true;
            }
            return false;
        }
        static bool IsActivated()
        {
            return false;
        }
        private static string GetCpuID()
        {
            // get a unique id for this PC
            string cpuID = "4685d7ifcuit775";// A random default string
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                cpuID = mo.Properties["processorID"].Value.ToString();
            }
            return cpuID;
        }
        // Hashing Sample from Microsoft
        private static string GetHash(string input)
        {
            SHA256 hashAlgorithm = SHA256.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        // Verify a hash against a string.
        private static bool VerifyHash(string input, string hash)
        {
            // Hash the input.
            var hashOfInput = GetHash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            return comparer.Compare(hashOfInput, hash) == 0;
        }     
    }
}
