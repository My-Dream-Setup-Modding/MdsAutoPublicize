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
                throw new Exception($"No Bepinex folder found???");

            var bepinPublicizeFolder = Path.Combine(bepinexFolder.FullName, "BepInEx.AssemblyPublicizer");
            var assemblyPublicizerExe = Path.Combine(bepinexFolder.FullName, "BepInEx.AssemblyPublicizer", "BepInEx.AssemblyPublicizer.Wrapper.exe");
            var managedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MDS_Data", "Managed");
            var publicizedFolderPath = Path.Combine(Directory.GetParent(assemblyPublicizerExe).FullName, $"Publicized");
            var assemblyCsharpFile = new FileInfo(Path.Combine(managedPath, $"Assembly-CSharp.dll"));

            var checkFilePath = Path.Combine(publicizedFolderPath, $"_{assemblyCsharpFile.Length}_{assemblyCsharpFile.CreationTime.Ticks}.txt");
            if (File.Exists(checkFilePath))
            {
                Logger.LogMessage($"Check file for auto publicizing found. Do nothing.");
                return;
            }

            Logger.LogWarning($"No publicized game files exist! Starting the auto publicizer.");

            var info = new ProcessStartInfo(assemblyPublicizerExe);
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            info.Arguments = $"\"{managedPath}\" \"{Path.GetFileName(checkFilePath)}\" ";
            var publicizer = Process.Start(info);
            ReadProcessOutput(publicizer.StandardOutput);
        }

        private void ReadProcessOutput(StreamReader processReader)
        {
            while (!processReader.EndOfStream)
                Logger.LogMessage(processReader.ReadLine());
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
