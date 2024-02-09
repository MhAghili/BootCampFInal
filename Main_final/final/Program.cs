using System.Reflection;
using final.Types;
using System.Threading.Tasks;
using BootcampReporter;

class Program
{

    static LogHistory log = new LogHistory();

    static List<string> Catagory = new List<string>();
    static List<IReportExtension> extensions = new List<IReportExtension>();
    static async Task Main()
    {
        await LoadExtensions("plugins");
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Bootcamp Reporter ::");
            Console.WriteLine("1. Run Report");
            Console.WriteLine("2. Manage Extensions");
            Console.WriteLine("3. View report history");
            Console.WriteLine("4. Exit");

            Console.Write("\nSelect option: ");
            int option;

            while (!int.TryParse(Console.ReadLine(), out option))
            {
                Console.Write("Invalid input. Please enter a valid option: ");
            }

            switch (option)
            {
                case 1:
                    RunReport();
                    break;
                case 2:
                    ManageExtensions();
                    break;
                case 3:
                    ViewReportHistory();
                    break;
                case 4:
                    Environment.Exit(0);
                    break;
            }
        }
    }

    static async Task LoadExtensions(string pluginFolderPath)
    {
        string[] pluginFiles = Directory.GetFiles(pluginFolderPath, "*.dll");

        // If  number of extentions more than 10 load in parallel :
        if (pluginFiles.Length > 10)
        {
            await LoadExtensionsInParallel(pluginFiles);
        }
        else
        {
            LoadExtensionsSequentially(pluginFiles);
        }
    }

    static async Task LoadExtensionsInParallel(string[] pluginFiles)
    {
        var loadingTasks = new List<Task>();

        foreach (string pluginFile in pluginFiles)
        {
            loadingTasks.Add(Task.Run(() => LoadExtensionsFromFile(pluginFile)));
        }

        await Task.WhenAll(loadingTasks);
    }

    static void LoadExtensionsSequentially(string[] pluginFiles)
    {
        foreach (string pluginFile in pluginFiles)
        {
            LoadExtensionsFromFile(pluginFile);
        }
    }

    static void LoadExtensionsFromFile(string pluginFile)
    {
        try
        {
            Assembly extensionAssembly = Assembly.LoadFrom(pluginFile);
            Type[] types = extensionAssembly.GetTypes();

            foreach (Type type in types)
            {
                var attribute = type.GetCustomAttribute(typeof(BootCampReportExtensionAttribute));
                if (attribute != null && typeof(IReportExtension).IsAssignableFrom(type))
                {
                    var ext = (IReportExtension)Activator.CreateInstance(type);
                    extensions.Add(ext);

                    if (!Catagory.Contains(ext.Catagory))
                    {
                        Catagory.Add(ext.Catagory);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    static void RunReport()
    {

        // -------------------------   Select a report   ------------------------- 

        Console.Clear();
        Console.WriteLine($"=== Bootcamp Reporter ::");

        //-----------------Catagory------------------

        Console.WriteLine("\n Select Catagory:");

        for (int i = 0; i < Catagory.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {Catagory[i]}");
        }
        Console.WriteLine($"{Catagory.Count + 1}. Back");




        int CatagoryIndex;
        while (!int.TryParse(Console.ReadLine(), out CatagoryIndex))
        {
            Console.Write("Invalid input");
        }


        if (CatagoryIndex == Catagory.Count + 1)
            return;


        Console.Clear();
        Console.WriteLine($"=== Bootcamp Reporter ::");
        Console.WriteLine("\nSelect a report: ");


        var reportNames = GetReportNames(CatagoryIndex);

        for (int i = 0; i < reportNames.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {reportNames[i]}");
        }
        Console.WriteLine($"{reportNames.Count + 1}. Back");

        Console.Write("\nSelect option: ");


        int reportIndex;
        while (!int.TryParse(Console.ReadLine(), out reportIndex))
        {
            Console.Write("Invalid input");
        }

        if (reportIndex == reportNames.Count + 1)
            return;

        string selectedReport = reportNames[reportIndex - 1];
        Console.Clear();
        Console.WriteLine($"=== Bootcamp Reporter ::");
        Console.WriteLine($"Result of '{selectedReport}':");

        // Execute selected report extension
        ExecuteReportExtension(selectedReport);

        Console.WriteLine("\n1. Back");
        Console.Write("\nSelect option: ");
        while (Console.ReadLine() != "1")
        {
            Console.Write("Invalid input. Please enter '1' to go back: ");
        }
    }

    static List<string> GetReportNames(int index)
    {
        List<string> reportNames = new List<string>();


        foreach (var ext in extensions)
        {
            if (ext.Enabled && ext.Catagory == Catagory[index - 1])
            {
                string reportName = ext.GetReportName();
                reportNames.Add(reportName);
            }
        }

        return reportNames;
    }


    static void ManageExtensions()
    {
        Console.Clear();
        Console.WriteLine("=== Bootcamp Reporter ::");
        Console.WriteLine("Manage Extensionsn:\n");

        for (int i = 0; i < extensions.Count; i++)
        {
            Console.WriteLine($"{extensions[i].GetType().Name} | {(extensions[i].Enabled ? "Enabled" : "Disabled")}");
        }

        Console.WriteLine("\n1. Enable/Disable Extension");
        Console.WriteLine("2. Back");

        Console.Write("\nSelect option: ");
        while (true)
        {
            int option;
            while (!int.TryParse(Console.ReadLine(), out option))
            {
                Console.Write("Invalid input. Please enter a valid option: ");
            }

            if (option == 2)
                return;

            if (option == 1)
            {
                EnableDisableExtension();
                return;
            }
        }
    }

    static void EnableDisableExtension()
    {

        Console.WriteLine("\nEnter the number of the extension: ");
        int extensionNumber;
        while (!int.TryParse(Console.ReadLine(), out extensionNumber))
        {
            Console.Write("Invalid input.");
        }

        var selectedExtension = extensions[extensionNumber - 1];
        selectedExtension.Enabled = !selectedExtension.Enabled;

        Console.WriteLine($"{selectedExtension.GetType().Name} is now {(selectedExtension.Enabled ? "Enabled" : "Disabled")}");

        while (true)
        {
            Console.WriteLine("\n1. Back");
            Console.Write("\nSelect option: ");
            if (Console.ReadLine() == "1")
                return;
        }

    }


    static void ViewReportHistory()
    {
        Console.Clear();
        Console.WriteLine("=== Bootcamp Reporter ::");
        Console.WriteLine("Report History:");
        Console.WriteLine(log.GetLog());
        Console.WriteLine("\n1. Back");
        Console.Write("\nSelect option: ");
        while (Console.ReadLine() != "1")
        {
            Console.Write("Invalid input");
        }
    }

    static void ExecuteReportExtension(string reportName)
    {
        Assembly extensionAssembly = Assembly.LoadFrom("plugins/Extentions.dll");

        Type[] types = extensionAssembly.GetTypes();

        foreach (var ext in extensions)
        {

            string Name = ext.GetReportName();

            if (Name == reportName)
            {
                string result = ext.Execute();
                log.AddLog(reportName);
                Console.WriteLine(result);

                break;
            }

        }
    }

}

