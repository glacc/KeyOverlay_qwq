// SPDX-License-Identifier: GPL-3.0-or-later

using System.Reflection;

namespace Glacc.KeyOverlay_qwq
{
    // Based on https://github.com/Friedchicken-42/KeyOverlay/blob/main/Config.cs

    internal class Config
    {
        public static Mutex configChangingMutex = new Mutex();

        public const string defaultConfigFileName = "config.ini";

        public static string? configFilePath;
        public static string configFileName = defaultConfigFileName;
        static string configFileFullPath = string.Empty;

        static Dictionary<string, Dictionary<string, string>> configSections = new Dictionary<string, Dictionary<string, string>>();
        public static Dictionary<string, Dictionary<string, string>> config
        {
            get => configSections;
        }

        static FileSystemWatcher? fileSystemWatcher;

        public static EventHandler<EventArgs>? onConfigFileChanged = null;

        public static void ReadConfigFile()
        {
            configChangingMutex.WaitOne();

            configSections.Clear();

            string[] linesOfConfigFile = File.ReadAllLines(configFileFullPath);

            Dictionary<string, string>? currentSection = null;
            string currentSectionName = string.Empty;
            foreach (string currentLineFull in linesOfConfigFile)
            {
                currentLineFull.Trim();

                // Remove comments
                int indexOfSemicolon = currentLineFull.IndexOf(';');
                int indexOfHash = currentLineFull.IndexOf("#");

                int indexOfStartOfComment;
                if (indexOfSemicolon >= 0 && indexOfHash >= 0)
                    indexOfStartOfComment = Math.Min(indexOfSemicolon, indexOfHash);
                else if (indexOfSemicolon >= 0)
                    indexOfStartOfComment = indexOfSemicolon;
                else if (indexOfHash >= 0)
                    indexOfStartOfComment = indexOfHash;
                else
                    indexOfStartOfComment = -1;

                string currentLine;
                if (indexOfStartOfComment >= 0)
                    currentLine = currentLineFull.Substring(0, indexOfStartOfComment).Trim();
                else
                    currentLine = currentLineFull.Trim();

                // Section
                if (currentLine.StartsWith('[') && currentLine.EndsWith(']'))
                {
                    currentSectionName = currentLine.Substring(1, currentLine.Length - 2);

                    if (!configSections.ContainsKey(currentSectionName))
                    {
                        currentSection = new Dictionary<string, string>();
                        configSections.Add(currentSectionName, currentSection);
                    }
                    else
                        currentSection = configSections[currentSectionName];

                    continue;
                }

                // Keys in section
                if (currentSection == null)
                    continue;

                string[] keyAndValue = currentLine.Split('=', StringSplitOptions.RemoveEmptyEntries);
                if (keyAndValue.Length == 2)
                {
                    string key = keyAndValue[0].Trim();
                    string value = keyAndValue[1].Trim();

                    if (!currentSection.ContainsKey(key))
                        currentSection.Add(key, value);
                    else
                        currentSection[key] = value;
                }
            }

            configChangingMutex.ReleaseMutex();

            if (fileSystemWatcher != null)
                fileSystemWatcher.EnableRaisingEvents = true;
        }

        static void OnConfigFileChangedInternal(object? sender, FileSystemEventArgs args)
        {
            // Wait until the config file actually changed
            Thread.Sleep(100);

            ReadConfigFile();

            onConfigFileChanged?.Invoke(null, EventArgs.Empty);
        }

        public static void SetConfigFile(string? newConfigFileName = null)
        {
            if (newConfigFileName == null)
            {
                configFileName = defaultConfigFileName;

                string assemblyPath = string.Join('/', Assembly.GetExecutingAssembly().Location.Split('\\', '/').SkipLast(1));
                configFilePath = assemblyPath;

                configFileFullPath = Path.Combine(assemblyPath, configFileName);
            }
            else
            {
                FileInfo fileInfo = new FileInfo(newConfigFileName);

                string[] splittedFilePath = fileInfo.FullName.Split('\\', '/');

                configFilePath = string.Join('/', splittedFilePath.SkipLast(1));
                configFileName = splittedFilePath.Last();

                configFileFullPath = Path.Combine(configFilePath, configFileName);
            }

            fileSystemWatcher?.Dispose();
            fileSystemWatcher = new FileSystemWatcher(configFilePath);
            fileSystemWatcher.Filter = configFileName;
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fileSystemWatcher.Changed += OnConfigFileChangedInternal;
        }
    }
}
