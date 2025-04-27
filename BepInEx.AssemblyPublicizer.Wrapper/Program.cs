namespace BepInEx.AssemblyPublicizer.Wrapper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var cmdStart = DateTime.Now;
            var managedFolder = args[0];
            var writeFileOnFinish = args[1];
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

            var dir = Directory.CreateDirectory(outputFolder);

            foreach (var file in Directory.EnumerateFiles(managedFolder))
            {
                var start = DateTime.Now;
                var fileName = Path.GetFileName(file);
                try
                {
                    BepInEx.AssemblyPublicizer.AssemblyPublicizer.Publicize(file,
                        Path.Combine(outputFolder, fileName));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while publicizing assembly.");
                    Console.WriteLine(ex);
                    continue;
                }

                var workTime = DateTime.Now - start;
                Console.WriteLine($"Publicized {workTime.TotalSeconds.ToString("0.00")} seconds for {fileName}");
            }

            Console.WriteLine($"Writing check file {writeFileOnFinish}");
            File.WriteAllText(Path.Combine(outputFolder, writeFileOnFinish),
                  "This file is used, to check if the Assembly-CSharp file has still the same MetaData as last game start.\n" +
                   "If this file gets deleted or does not fit anymore, the publicizing process gets started.");

            var cmdWorkTime = DateTime.Now - cmdStart;
            Console.WriteLine($"Completed publicizing all game files in {cmdWorkTime.TotalSeconds.ToString("0.00")} seconds.");
        }
    }
}
