using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Security.Cryptography.X509Certificates;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("--------- Start Check New Version ATM NCR -----------");
        string directory = obtenerDirectoryWithOS();
        string currentVersion = getCurrentVersion(directory);

        string[] instaladores = Directory.GetFiles(directory);
        instaladores = Array.FindAll(instaladores, i => i.Contains(".exe") == true && i.Contains("ATM-") == true).ToArray();
            
        List<int> numberVersions = new List<int>();
        numberVersions = getListNumberVersions(instaladores, directory);

        int mayorVersion = numberVersions.Max();
        int indexMayorVersion = numberVersions.IndexOf(mayorVersion);

        if (currentVersion != "ROLLBACK")
        {
            currentVersion = currentVersion.Substring(currentVersion.IndexOf("v") + 1);
            int currentVersionNumber = int.Parse(currentVersion.Substring(0, currentVersion.Length - 4));

            Console.WriteLine("[INFO] La version instalada es la: " + currentVersion);
            Console.WriteLine("[INFO] La version mas actual disponible para instalar es la: " + mayorVersion);

            if (currentVersionNumber < mayorVersion)
            {
                Console.Write("[INFO] Se ha encontrado una version superior a la instalada. ");
                Console.Write("[INFO] Iniciando proceso de actualizacion de version:");
                Process.Start(instaladores[indexMayorVersion]);
            }
            else if (currentVersionNumber == mayorVersion)
            {
                Console.WriteLine("[INFO] Tiene instalada la version mas actual disponible");
            }
            else
            {
                Console.WriteLine("[WARN] La version instalada es superior a las disponibles. Por favor, verificar el recurso compartido.");
            }
            Console.WriteLine("--------- End Check New Version ATM NCR -----------");
        }
        else
        {
            Console.WriteLine("[INFO] El cajero tiene instalada una version de Rollback");
            Console.WriteLine("[INFO] Se procedera a instalarle la ultima version disponible");
            Process.Start(instaladores[indexMayorVersion]);
        }
    }

    public static string obtenerDirectoryWithOS()
    {
        string direct = "";
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            Version osVersion = Environment.OSVersion.Version;
            if (osVersion.Major == 10)
            {
                Console.WriteLine("Windows 10");
                direct = "C:/Program Files (x86)/NCR APTRA/Advance NDC/instaladores/";
            }
            else if (osVersion.Major == 6 && osVersion.Minor == 1)
            {
                Console.WriteLine("Windows 7");
                direct = "C:/Program Files/NCR APTRA/Advance NDC/instaladores/";
            }
        }
        return direct;
    }

    public static string getCurrentVersion(string directory)
    {
        string[] installConfig = File.ReadAllLines(directory + "/InstallConfig.ini");
        string currentVersion = installConfig[1];
        if (currentVersion.StartsWith("filename=") && !currentVersion.Contains("ROLLBACK"))
        {
            currentVersion = currentVersion.Substring(9);
        }
        else if (currentVersion.Contains("ROLLBACK"))
        {
            currentVersion = "ROLLBACK";
        }
        return currentVersion;
    }

    public static List<int> getListNumberVersions(string[] instaladores, string directory)
    {
        List<int> numberVersions = new List<int>();
        foreach (string path in instaladores)
        {
            string newPath = path.Substring(0, path.Length - 4);
            string number = newPath.Substring(directory.Length + 5);
            numberVersions.Add(int.Parse(number));
        }
        return numberVersions;
    }
}