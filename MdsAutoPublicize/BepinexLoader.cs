using BepInEx;
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace MdsAutoPublicize
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class BepinexLoader : BaseUnityPlugin
    {
        public const string
            MODNAME = "MdsAutoPublicize",
            AUTHOR = "Edsil",
            GUID = AUTHOR + "." + MODNAME,
            VERSION = "1.0.1";

        public void Awake()
        {
            this.gameObject.SetActive(false);

            var pluginPath = new DirectoryInfo(Path.GetDirectoryName(Info.Location));
            if (!TryFindParent(pluginPath, "BepInEx", out var bepinexFolder))
                throw new Exception($"No Bepinex folder found???");

            //Output Path, where the publicized assemblies lands for this BepInEx installation.
            var bepinPublicizedFolder = Path.Combine(bepinexFolder.FullName, "BepInEx.AssemblyPublicizer.Publicized");
            //Using Parent.Fullname since Info.Loca
            var assemblyPublicizerExe = Path.Combine(pluginPath.FullName, "BepInEx.AssemblyPublicizer.Wrapper", "BepInEx.AssemblyPublicizer.Wrapper.exe");
            var managedPath = Path.Combine(Application.dataPath, "Managed");
            var assemblyCsharpFile = new FileInfo(Path.Combine(managedPath, $"Assembly-CSharp.dll"));

            var checkFilePath = Path.Combine(bepinPublicizedFolder, $"_{assemblyCsharpFile.Length}_{assemblyCsharpFile.CreationTime.Ticks}.txt");
            if (File.Exists(checkFilePath))
            {
                Logger.LogMessage($"Check file for auto publicizing found. Do nothing.");
                return;
            }

            Logger.LogWarning($"No publicized game files exist! Starting the auto publicizer.");
            try
            {
                var info = new ProcessStartInfo(assemblyPublicizerExe);
                info.RedirectStandardOutput = true;
                info.UseShellExecute = false;
                info.CreateNoWindow = true;
                info.Arguments = $"\"{managedPath}\" \"{Path.GetFileName(checkFilePath)}\" \"{bepinPublicizedFolder}\" ";
                var publicizer = Process.Start(info);
                ReadProcessOutput(publicizer.StandardOutput);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Something went wrong, while starting the publicizer process and reading output:\n{ex}");
            }
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
