﻿using System.Globalization;

using DataImportClient.Scripts;





namespace DataImportClient.Modules
{
    internal class Photovoltaic
    {
        private const string _currentSection = "ModulePhotovoltaic";

        private ModuleState _moduleState;
        private static bool _serviceRunning;
        private static int _errorCount;

        private static int _navigationXPosition = 1;
        private static readonly int _countOfMenuOptions = 4;

        private static string _dateOfLastImport = string.Empty;
        private static string _dateOfLastLogFileEntry = string.Empty;

        private static string _formattedErrorCount = string.Empty;
        private static string _formattedServiceRunning = string.Empty;
        private static string _formattedLastLogFileEntry = string.Empty;



        internal ModuleState State
        {
            get => _moduleState;
            set
            {
                if (_moduleState != value)
                {
                    ActivityLogger.Log(_currentSection, $"Module state changed from '{_moduleState}' to '{value}'.");
                    _moduleState = value;

                    OnStateChange();
                }
            }
        }

        internal event EventHandler? StateChanged;

        protected virtual void OnStateChange()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        internal int ErrorCount
        {
            get => _errorCount;
        }



        internal Photovoltaic()
        {
            _moduleState = ModuleState.Running;
            _errorCount = 0;
            _serviceRunning = true;

            _dateOfLastImport = DateTime.Now.ToString("dd.MM.yyyy - HH:mm:ss");
            _dateOfLastLogFileEntry = DateTime.Now.ToString("dd.MM.yyyy - HH:mm:ss");
        }



        internal async Task Main()
        {
            ActivityLogger.Log(_currentSection, "Entering module 'Photovoltaic'.");



            Console.Clear();

        LabelDrawUi:

            Console.SetCursorPosition(0, 4);



            ActivityLogger.Log(_currentSection, "Formatting menu variables.");

            FormatMenuVariables();

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

                case ConsoleKey.Escape:
                    ActivityLogger.Log(_currentSection, "Returning to the main menu via 'ESC'.");
                    return;

                case ConsoleKey.Backspace:
                    ActivityLogger.Log(_currentSection, "Returning to the main menu via 'BACKSPACE'.");
                    return;

                default:
                    break;
            }



            if (pressedKey != ConsoleKey.Enter)
            {
                goto LabelDrawUi;
            }



            switch (_navigationXPosition)
            {
                case 1:
                    break;

                case 2:
                    break;

                case 3:
                    break;

                case 4:
                    ActivityLogger.Log(_currentSection, "Returning to the main menu via selection.");
                    return;
            }



            Console.Clear();
            goto LabelDrawUi;
        }

        private static void DisplayMenu()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("              ┏┓┓          ┓   •                               ");
            Console.WriteLine("              ┃┃┣┓┏┓╋┏┓┓┏┏┓┃╋┏┓┓┏                              ");
            Console.WriteLine("              ┣┛┛┗┗┛┗┗┛┗┛┗┛┗┗┗┻┗┗                              ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("             ───────────────────────                           ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                                                               ");
            Console.WriteLine("                                                               ");
            Console.WriteLine("             │   Last import:  {0}   │                         ", _dateOfLastImport);
            Console.WriteLine("             └─────────────────────────────────────────┘       ");
            Console.WriteLine("                                                               ");
            Console.WriteLine("                                                               ");
            Console.WriteLine("             ┌ Options                             State       ");
            Console.WriteLine("             └──────────────────────┐              ┌───┐       ");
            Console.WriteLine("             {0} Open log file                     │ {1}       ", $"[\u001b[91m{(_navigationXPosition == 1 ? ">" : " ")}\u001b[97m]", _formattedLastLogFileEntry);
            Console.WriteLine("             {0} {1}                     │ {2}       ", $"[\u001b[91m{(_navigationXPosition == 2 ? ">" : " ")}\u001b[97m]", $"{(_serviceRunning ? "Stop service " : "Start service")}", _formattedServiceRunning);
            Console.WriteLine("             {0} Clear errors                      │ {1}       ", $"[\u001b[91m{(_navigationXPosition == 3 ? ">" : " ")}\u001b[97m]", _formattedErrorCount);
            Console.WriteLine("                                                   └───┘       ");
            Console.WriteLine("                                                               ");
            Console.WriteLine("             ┌ Application                                     ");
            Console.WriteLine("             └──────────────────────┐                          ");
            Console.WriteLine("             {0} MainMenu                                      ", $"[\u001b[91m{(_navigationXPosition == 4 ? ">" : " ")}\u001b[97m]");
        }

        private static void FormatMenuVariables()
        {
            _formattedErrorCount = "\u001b[92m√\u001b[97m │ \u001b[92mCleared\u001b[97m";
            _formattedServiceRunning = "\u001b[92m√\u001b[97m │ \u001b[92mRunning\u001b[97m";

            if (_errorCount > 0)
            {
                _formattedErrorCount = $"\u001b[91mx\u001b[97m │ \u001b[91m{_errorCount} {(_errorCount > 1 ? "Errors" : "Error")}\u001b[97m";
            }

            if (_serviceRunning == false)
            {
                _formattedServiceRunning = "\u001b[93mo\u001b[97m │ \u001b[93mStopped\u001b[97m";
            }



            if (!DateTime.TryParseExact(_dateOfLastLogFileEntry, "dd.MM.yyyy - HH:mm:ss", null, DateTimeStyles.None, out DateTime providedDateTime))
            {
                _formattedLastLogFileEntry = "\u001b[96m?\u001b[97m │ \u001b[96mUnknown\u001b[97m";
            }

            if (providedDateTime > DateTime.Now)
            {
                _formattedLastLogFileEntry = "\u001b[96m?\u001b[97m │ \u001b[96mUnknown\u001b[97m";
            }



            TimeSpan difference = DateTime.Now - providedDateTime;

            if (difference.TotalMinutes < 30)
            {
                _formattedLastLogFileEntry = $"\u001b[92m√\u001b[97m │ Updated at '\u001b[92m{_dateOfLastLogFileEntry}\u001b[97m'";

            }
            else if (difference.TotalMinutes >= 30 && difference.TotalMinutes < 60)
            {
                _formattedLastLogFileEntry = $"\u001b[93mo\u001b[97m │ Updated at '\u001b[93m{_dateOfLastLogFileEntry}\u001b[97m'";
            }
            else
            {
                _formattedLastLogFileEntry = $"\u001b[91mx\u001b[97m │ Updated at '\u001b[91m{_dateOfLastLogFileEntry}\u001b[97m'";
            }
        }
    }
}