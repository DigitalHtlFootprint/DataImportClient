﻿using DataImportClient.Modules;
using DataImportClient.Scripts;





namespace DataImportClient
{
    enum ModuleState
    {
        Running = 1,
        Stopped = 2,
        Unkown = 3,
        Error = 4
    }



    internal class MainMenu
    {
        private const string _currentSection = "MainMenu";

        private static int _navigationXPosition = 1;
        private static readonly int _countOfMenuOptions = 5;

        private static readonly Weather _moduleWeather = new();
        private static readonly Electricity _moduleElectricity = new();
        private static readonly DistrictHeat _moduleDistrictHeat = new();
        private static readonly Photovoltaic _modulePhotovoltaic = new();

        private static string _stateWeather = string.Empty;
        private static string _stateElectricity = string.Empty;
        private static string _stateDistrictHeat = string.Empty;
        private static string _statePhotovoltaic = string.Empty;



        internal static async Task Main()
        {
            ActivityLogger.Log(_currentSection, "Entering main menu.");



            Console.Clear();

        LabelDrawUi:

            Console.SetCursorPosition(0, 4);

            ActivityLogger.Log(_currentSection, "Formatting module states.");

            _stateWeather = FormatModuleStates(_moduleWeather.State, _moduleWeather.ErrorCount);
            _stateElectricity = FormatModuleStates(_moduleElectricity.State, _moduleElectricity.ErrorCount);
            _stateDistrictHeat = FormatModuleStates(_moduleDistrictHeat.State, _moduleDistrictHeat.ErrorCount);
            _statePhotovoltaic = FormatModuleStates(_modulePhotovoltaic.State, _modulePhotovoltaic.ErrorCount);



            ActivityLogger.Log(_currentSection, "Starting to draw the main menu.");

            DisplayMenu();

            ActivityLogger.Log(_currentSection, "Displayed main menu, waiting for key input.");



            ConsoleKey pressedKey = Console.ReadKey(true).Key;

            switch (pressedKey)
            {
                case ConsoleKey.DownArrow:
                    if (_navigationXPosition + 1 <= _countOfMenuOptions)
                    {
                        _navigationXPosition += 1;
                        ActivityLogger.Log(_currentSection, $"Changed menu option from '{_navigationXPosition - 1}' to '{_navigationXPosition}'.");
                    }
                    break;

                case ConsoleKey.UpArrow:
                    if (_navigationXPosition - 1 >= 1)
                    {
                        _navigationXPosition -= 1;
                        ActivityLogger.Log(_currentSection, $"Changed menu option from '{_navigationXPosition + 1}' to '{_navigationXPosition}'.");
                    }
                    break;

                case ConsoleKey.Backspace:
                    return;

                case ConsoleKey.Escape:
                    return;

                default:
                    break;
            }



            if (pressedKey != ConsoleKey.Enter)
            {
                goto LabelDrawUi;
            }



            ActivityLogger.Log(_currentSection, $"Switching to module '{_navigationXPosition}'.");

            switch (_navigationXPosition)
            {
                case 1:
                    await _moduleWeather.Main();
                    break;

                case 2:
                    await _moduleElectricity.Main();
                    break;

                case 3:
                    await _moduleDistrictHeat.Main();
                    break;

                case 4:
                    await _modulePhotovoltaic.Main();
                    break;

                case 5:
                    // TODO: Call main method of the selected module
                    break;
            }



            ActivityLogger.Log(_currentSection, $"Redrawing main menu after returning from selected module.");

            Console.Clear();
            goto LabelDrawUi;
        }

        private static void DisplayMenu()
        {
            Console.WriteLine("              {0}                                              ", "\u001b[91m┳┓•  •   ┓ \u001b[97m┓┏┏┳┓┓  \u001b[91m┏┓         •   \u001b[97m");
            Console.WriteLine("              {0}                                              ", "\u001b[91m┃┃┓┏┓┓╋┏┓┃ \u001b[97m┣┫ ┃ ┃  \u001b[91m┣ ┏┓┏┓╋┏┓┏┓┓┏┓╋\u001b[97m");
            Console.WriteLine("              {0}                                              ", "\u001b[91m┻┛┗┗┫┗┗┗┻┗ \u001b[97m┛┗ ┻ ┗┛ \u001b[91m┻ ┗┛┗┛┗┣┛┛ ┗┛┗┗\u001b[97m");
            Console.WriteLine("              {0}                                              ", "\u001b[91m    ┛      \u001b[97m        \u001b[91m       ┛       \u001b[97m");
            Console.WriteLine("             ─────────────────────────────────────────         ");
            Console.WriteLine("                                                               ");
            Console.WriteLine("                                                               ");
            Console.WriteLine("                                                               ");
            Console.WriteLine("             ┌ Modules                           State         ");
            Console.WriteLine("             └────────────────┐                  ┌───┐         ");
            Console.WriteLine("             {0} Weather                         │ {1}         ", $"[\u001b[91m{(_navigationXPosition == 1 ? ">" : " ")}\u001b[97m]", _stateWeather);
            Console.WriteLine("             {0} Electricity                     │ {1}         ", $"[\u001b[91m{(_navigationXPosition == 2 ? ">" : " ")}\u001b[97m]", _stateElectricity);
            Console.WriteLine("             {0} DistrictHeat                    │ {1}         ", $"[\u001b[91m{(_navigationXPosition == 3 ? ">" : " ")}\u001b[97m]", _stateDistrictHeat);
            Console.WriteLine("             {0} Photovoltaic                    │ {1}         ", $"[\u001b[91m{(_navigationXPosition == 4 ? ">" : " ")}\u001b[97m]", _statePhotovoltaic);
            Console.WriteLine("                                                 └───┘         ");
            Console.WriteLine("                                                               ");
            Console.WriteLine("                                                               ");
            Console.WriteLine("             ┌ Application                                     ");
            Console.WriteLine("             └────────────────┐                                ");
            Console.WriteLine("             {0} Settings                                      ", $"[\u001b[91m{(_navigationXPosition == 5 ? ">" : " ")}\u001b[97m]");
        }

        private static string FormatModuleStates(ModuleState moduleState, int errorCount)
        {
            string formattedState = "\u001b[96m?\u001b[97m │ \u001b[96mUnknown\u001b[97m";

            switch (moduleState)
            {
                case ModuleState.Running:
                    formattedState = "\x1B[92m√\x1B[97m │ \u001b[92mRunning\u001b[97m";
                    break;

                case ModuleState.Stopped:
                    formattedState = "\x1B[93mo\x1B[97m │ \u001b[93mStopped\u001b[97m";
                    break;

                case ModuleState.Error:
                    formattedState = $"\x1B[91mx\x1B[97m │ \u001b[91m{errorCount} {(errorCount > 1 ? "Errors" : "Error")} \u001b[97m";
                    break;

                default:
                    break;
            }

            return formattedState;
        }
    }
}