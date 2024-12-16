﻿using System.Text;

using DataImportClient.Scripts;
using DataImportClient.Ressources;

using Newtonsoft.Json.Linq;





namespace DataImportClient
{
    internal class EntryPoint
    {
        private const string _currentSection = "EntryPoint";

        private static readonly ApplicationSettings.Paths _appPaths = new();
        private static readonly ApplicationSettings.Runtime _appRuntime = new();
        


        static async Task Main()
        {
            string appVersion = _appRuntime.appVersion;
            string appRelease = _appRuntime.appRelease;

            ActivityLogger.Log(_currentSection, string.Empty, true);
            ActivityLogger.Log(_currentSection, "Starting DataImportClient (C) Made in Austria");
            ActivityLogger.Log(_currentSection, $"Version '{appVersion}' | Release '{appRelease}'");



            ActivityLogger.Log(_currentSection, "Trying to enable support for ANSI escape sequence.");
            (bool ansiSupportEnabled, Exception occuredError) = ConsoleHelper.EnableAnsiSupport();

            if (ansiSupportEnabled == false)
            {
                ActivityLogger.Log(_currentSection, "[ERROR] Failed to enable ANSI support.");
                ActivityLogger.Log(_currentSection, occuredError.Message, true);

                Console.SetCursorPosition(0, 4);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("             WARNING\r\n");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("             Failed to enable support for ANSI escape sequences.");
                Console.WriteLine("             This will have side effects on the coloring within the console.\r\n");
                Console.WriteLine("             Please read the manual on how to fix this error!");

                Thread.Sleep(5000);
            }
            else
            {
                ActivityLogger.Log(_currentSection, "Successfully enabled ANSI support!");
            }



            ActivityLogger.Log(_currentSection, "Searching for all required folders/files.");
            (bool successfullyCreated, occuredError) = await CreateDiskFolderStructure();

            if (successfullyCreated == false)
            {
                ActivityLogger.Log(_currentSection, "[ERROR] Failed to create required folder/file structure.");
                ActivityLogger.Log(_currentSection, occuredError.Message, true);

                Console.SetCursorPosition(0, 4);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("             WARNING\r\n");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("             Failed to create the required folder/file structure");
                Console.WriteLine("             This structure is cruicial for the application to work.\r\n");
                Console.WriteLine("             Please read the manual on how to fix this error!");

                Thread.Sleep(5000);

                Environment.Exit(0);
            }

            ActivityLogger.Log(_currentSection, "All required folders/files exist at the correct path!");



            Console.CursorVisible = false;
            Console.Title = "DataImportClient";
            Console.OutputEncoding = Encoding.UTF8;
            


            await MainMenu.Main();
            


            ActivityLogger.Log(_currentSection, "Shutting down DataImportClient ...");

            Environment.Exit(0);
        }

        private static async Task<(bool successfullyCreated, Exception occuredError)> CreateDiskFolderStructure()
        {
            string appDataFolder = _appPaths.appDataFolder;
            string configurationFile = _appPaths.configurationFile;

            List<string> foldersToCreate =
            [
                _appPaths.modulesFolder,
                _appPaths.weatherFolder,
                _appPaths.weatherFaultyFilesFolder,
                _appPaths.electricityFolder,
                _appPaths.electricityFaultyFilesFolder,
                _appPaths.districtHeatFolder,
                _appPaths.districtHeatFaultyFilesFolder,
                _appPaths.photovoltaicFolder,
                _appPaths.photovoltaicFaultyFilesFolder
            ];



            foreach (string folder in foldersToCreate)
            {
                try
                {
                    if (Directory.Exists(folder) == false)
                    {
                        Directory.CreateDirectory(folder);

                        ActivityLogger.Log(_currentSection, $"Created folder: {folder.Replace(appDataFolder, ".")}");
                    }

                    await Task.Delay(50);
                }
                catch (Exception exception)
                {
                    return (false, exception);
                }
                
            }



            try
            {
                if (File.Exists(configurationFile) == false)
                {
                    JObject appConfiguration = [];

                    JObject modules = new()
                    {
                        ["weather"] = new JObject()
                        {
                            ["apiUrl"] = "https://urlTo.TheAPI.provider",
                            ["apiKey"] = "tableNameforImport",
                            ["apiInterval"] = "howLongToWaitBeforeCallingApi"
                        },
                        ["electricity"] = new JObject()
                        {
                            ["sourceFilePath"] = "path/to/source/file.csv",
                            ["dbTableName"] = "tableNameforImport",
                            ["refreshTimeInSeconds"] = "intervalInWhichTheSourceDataArrives"
                        },
                        ["districtHeat"] = new JObject()
                        {
                            ["sourceFilePath"] = "path/to/source/file.csv",
                            ["dbTableName"] = "tableNameforImport",
                            ["refreshTimeInSeconds"] = "intervalInWhichTheSourceDataArrives"
                        },
                        ["photovoltaic"] = new JObject()
                        {
                            ["sourceFilePath"] = "path/to/source/file.csv",
                            ["dbTableName"] = "tableNameforImport",
                            ["refreshTimeInSeconds"] = "intervalInWhichTheSourceDataArrives"
                        }
                    };

                    appConfiguration["modules"] = modules;
                    appConfiguration["sql"] = new JObject()
                    {
                        ["connectionString"] = "theConnectionStringForTheImportDatabase"
                    };
                    appConfiguration["emailsToAlert"] = new JArray();



                    await File.WriteAllTextAsync(configurationFile, appConfiguration.ToString());

                    ActivityLogger.Log(_currentSection, $"Created file: {configurationFile.Replace(appDataFolder, ".")}");
                }
            }
            catch (Exception exception)
            {
                return (false, exception);
            }
            


            return (true, new Exception());
        }
    }
}