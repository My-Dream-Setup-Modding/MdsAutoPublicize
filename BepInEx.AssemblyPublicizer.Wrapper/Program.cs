namespace BepInEx.AssemblyPublicizer.Wrapper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var cmdStart = DateTime.Now;
            var managedFolder = args[0];
            var outputFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Publicized");

            Console.WriteLine($"Start publicizing all files from {managedFolder} into {outputFolder}");
            if (Directory.Exists(outputFolder))
            {
                Console.WriteLine($"Removing old publicized gamefiles.");
                try
                {
                    Directory.Delete(outputFolder, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed deleting existing publicized files, stop execution.");
                    Console.WriteLine($"Exception:\n{ex.ToString()}");
                    return;
                }
                Console.WriteLine("============================================");
            }

            Directory.CreateDirectory(outputFolder);

            foreach (var file in Directory.EnumerateFiles(managedFolder))
            {
                var start = DateTime.Now;
                var fileName = Path.GetFileName(file);

                BepInEx.AssemblyPublicizer.AssemblyPublicizer.Publicize(file,
                    Path.Combine(outputFolder, fileName));

                var workTime = DateTime.Now - start;
                Console.WriteLine($"Worked {workTime.TotalSeconds.ToString("0.00")} seconds to publicized {fileName}");
            }

            var cmdWorkTime = DateTime.Now - cmdStart;
            Console.WriteLine($"Completed publicizing all game files in {cmdWorkTime.TotalSeconds.ToString("0.00")}");
            Console.WriteLine("Closing in 5 seconds...");

            Thread.Sleep(5000);
        }
    }
}
