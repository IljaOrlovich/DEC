using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class DESExample
{
    static void Main()
    {
        bool repeat = true;

        while (repeat)
        {
            Console.WriteLine("Pasirinkite veiksmą (1 - šifravimas, 2 - dešifravimas, 0 - baigti):");
            int choice = int.Parse(Console.ReadLine());

            if (choice == 0)
            {
                repeat = false;
                continue;
            }

            Console.WriteLine("Įveskite slaptažodį:");
            string key = Console.ReadLine();

            string resultText = "";

            if (choice == 1)
            {
                Console.WriteLine("Įveskite pradinį tekstą:");
                string plaintext = Console.ReadLine();

                Console.WriteLine("Pasirinkite šifravimo modą (ECB, CBC, CFB):");
                string mode = Console.ReadLine();

                resultText = Encrypt(plaintext, key, mode);
                Console.WriteLine("Užšifruotas tekstas: " + resultText);

                Console.WriteLine("Ar norite išsaugoti užšifruotą tekstą į failą? (y/n):");
                if (Console.ReadLine().ToLower() == "y")
                {
                    Console.WriteLine("Įveskite failo pavadinimą:");
                    string filePath = Console.ReadLine();
                    File.WriteAllText(filePath, resultText);
                    Console.WriteLine("Užšifruotas tekstas išsaugotas į failą: " + filePath);
                }
            }
            else if (choice == 2)
            {
                Console.WriteLine("Įveskite failo pav:");
                string filePath = Console.ReadLine();
                string cipherText = File.ReadAllText(filePath);
                Console.WriteLine("Pasirinkite šifravimo modą (ECB, CBC, CFB):");
                string mode = Console.ReadLine();

                resultText = Decrypt(cipherText, key, mode);
                Console.WriteLine("Dešifruotas tekstas: " + resultText);
            }
        }
    }

    static string Encrypt(string plainText, string key, string mode)
    {
        using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
        {
            des.Key = Encoding.UTF8.GetBytes(key);
            des.IV = Encoding.UTF8.GetBytes(key);

            SetCipherMode(des, mode);

            using (ICryptoTransform encryptor = des.CreateEncryptor())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(plainText);
                        cs.Write(bytes, 0, bytes.Length);
                        cs.FlushFinalBlock();
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
    }

    static string Decrypt(string cipherText, string key, string mode)
    {
        using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
        {
            des.Key = Encoding.UTF8.GetBytes(key);
            des.IV = Encoding.UTF8.GetBytes(key);

            SetCipherMode(des, mode);

            using (ICryptoTransform decryptor = des.CreateDecryptor())
            {
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cs))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }

    static void SetCipherMode(SymmetricAlgorithm algorithm, string mode)
    {
        switch (mode.ToUpper())
        {
            case "ECB":
                algorithm.Mode = CipherMode.ECB;
                break;
            case "CBC":
                algorithm.Mode = CipherMode.CBC;
                break;
            case "CFB":
                algorithm.Mode = CipherMode.CFB;
                break;
            default:
                throw new ArgumentException("Nežinomas šifravimo modas");
        }
    }
}
