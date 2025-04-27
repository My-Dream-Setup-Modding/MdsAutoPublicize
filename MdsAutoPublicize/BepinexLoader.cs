using BepInEx;
using System;
using System.Diagnostics;
using System.IO;

namespace MdsAutoPublicize
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class BepinexLoader : BaseUnityPlugin
    {
        public const string
            MODNAME = "MdsAutoPublicize",
            AUTHOR = "Edsil",
            GUID = AUTHOR + "." + MODNAME,
            VERSION = "1.0.0";

        public void Awake()
        {
            this.gameObject.SetActive(false);

            var pluginPath = new DirectoryInfo(Info.Location);
            if (!TryFindParent(pluginPath, "BepInEx", out var bepinexFolder))
                throw new NullReferenceException($"No Bepinex folder found!");

            var assemblyPublicizerExe = Path.Combine(bepinexFolder.FullName, "BepInEx.AssemblyPublicizer", "BepInEx.AssemblyPublicizer.Wrapper.exe");
            var managedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MDS_Data", "Managed");
            var publicizedFolder = Path.Combine(Directory.GetParent(assemblyPublicizerExe).FullName, $"Publicized");

            if (Directory.Exists(publicizedFolder))
            {
                Logger.LogMessage($"Check file for auto publicizing found. Do nothing.");
                return;
            }

            Logger.LogWarning($"No publicized game files exist! Starting the process, to generate them.");

            var info = new ProcessStartInfo(assemblyPublicizerExe);
            info.RedirectStandardOutput = true;
            info.Arguments = $"\"managedPath\"";
            var publicizer = Process.Start(info);

            publicizer.OutputDataReceived += Received;
            publicizer.Start();
            publicizer.Exited += Exited;

            return;

            var publicizedPath = Path.Combine(managedPath, "Publicized");
            var assemblyCsharpFile = new FileInfo(Path.Combine(managedPath, $"Assembly-CSharp.dll"));

            var length = assemblyCsharpFile.Length;
            var create = assemblyCsharpFile.CreationTime.Ticks;

            Directory.CreateDirectory(publicizedPath);
            var checkFilePath = Path.Combine(publicizedPath, $"_{length}_{create}.txt");
            var checkFile = new FileInfo(checkFilePath);

            if (checkFile.Exists)
            {
                Logger.LogMessage($"Check file for auto publicizing found. Do nothing.");
                return;
            }

            Logger.LogWarning($"No publicized game files exist! Creating them, this can take some time ...");
            try
            {
                foreach (var oldPublicizedFile in Directory.EnumerateFiles(publicizedPath))
                {
                    Logger.LogMessage($"Removing old publicized file: {Path.GetFileName(oldPublicizedFile)}.");
                    File.Delete(oldPublicizedFile);
                }

                Logger.LogMessage("==================================================");
                Logger.LogMessage("Removing of the old publicized files finished, starting generating new ones.");
                Logger.LogMessage("==================================================");

                foreach (var gameFile in Directory.EnumerateFiles(managedPath))
                {
                    Logger.LogMessage($"Publicizing {Path.GetFileName(gameFile)}.");
                    BepInEx.AssemblyPublicizer.AssemblyPublicizer.Publicize(gameFile,
                        Path.Combine(publicizedPath, Path.GetFileName(gameFile)));
                }

                Logger.LogMessage($"Create check file.");
                File.WriteAllText(checkFilePath,
                    "This file is used, to check if the Assembly-CSharp file has still the same MetaData as last game start.\n" +
                    "If this file gets deleted or does not fit anymore, the publicizing process gets started.");

                Logger.LogMessage("==================================================");
                Logger.LogMessage("Finished generating publicized game files.");

            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Failed during creation of new publicized gam6e files.");
                throw ex;
            }
            //Directory.EnumerateFiles(managedPath, "*.dll");


            //BepInEx.AssemblyPublicizer.AssemblyPublicizer
        }

        private void Received(object sender, DataReceivedEventArgs e)
        {
            Logger.LogMessage(e.Data);
        }

        private void Exited(object sender, EventArgs e)
        {
            Logger.LogMessage($"Game library publiciser closed.");
            var proc = sender as Process;
            proc.OutputDataReceived -= Received;
            proc.Exited -= Exited;
            proc.Dispose();
        }

        private bool TryFindParent(DirectoryInfo dir, string findName, out DirectoryInfo info)
        {
            if (dir is null)
            {
                info = null;
                return false;
            }

            if (dir.Name != findName)
                return TryFindParent(dir.Parent, findName, out info);

            info = dir;
            return true;
        }
    }
}
