using BepInEx;
using System;
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
            VERSION = "1.0.2";

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

            try
            {
                Run(managedPath, Path.GetFileName(checkFilePath), bepinPublicizedFolder);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Something went wrong, while starting the publicizer:\n{ex}");
            }
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

        private void Run(string managedFolder, string writeFileOnFinish, string outputFolder)
        {
            var cmdStart = DateTime.Now;

            Logger.LogMessage($"Start publicizing all files from {managedFolder} into {outputFolder}");
            if (Directory.Exists(outputFolder))
            {
                Logger.LogMessage($"Removing old publicized gamefiles.");
                try
                {
                    Directory.Delete(outputFolder, true);
                }
                catch (Exception ex)
                {
                    Logger.LogMessage($"Failed deleting existing publicized files, stop execution.");
                    Logger.LogMessage($"Exception:\n{ex.ToString()}");
                    return;
                }

                Logger.LogMessage("============================================");
            }

            var dir = Directory.CreateDirectory(outputFolder);

            foreach (var file in Directory.EnumerateFiles(managedFolder))
            {
                var start = DateTime.Now;
                var fileName = Path.GetFileName(file);
                try
                {
                    if (fileName == "ICSharpCode.SharpZipLib.dll")
                    {
                        File.Copy(file, Path.Combine(outputFolder, fileName));
                    }
                    else
                    {
                        BepInEx.AssemblyPublicizer.AssemblyPublicizer.Publicize(file,
                        Path.Combine(outputFolder, fileName));
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error while publicizing assembly.\n{ex}");
                    continue;
                }

                var workTime = DateTime.Now - start;
                Logger.LogMessage($"Publicized {workTime.TotalSeconds.ToString("0.000")} seconds for {fileName}");
            }

            Logger.LogMessage($"Writing check file {writeFileOnFinish}");
            File.WriteAllText(Path.Combine(outputFolder, writeFileOnFinish),
                  "This file is used, to check if the Assembly-CSharp file has still the same MetaData as last game start.\n" +
                   "If this file gets deleted or does not fit anymore, the publicizing process gets started.");

            var cmdWorkTime = DateTime.Now - cmdStart;
            Logger.LogMessage($"Completed publicizing all game files in {cmdWorkTime.TotalSeconds.ToString("0.000")} seconds.");
        }
    }
}
