using System;
using TwitchShoppingNetworkLogger.Auditor.Impl;
using TwitchShoppingNetworkLogger.Auditor.Interfaces;
using TwitchShoppingNetworkLogger.Config;
using TwitchShoppingNetworkLogger.Excel;

namespace TwitchShoppingNetworkLogger.Console
{
    public class Program
    {
        private static IUserRepository _userRepository;
        private static IAuditorRegistry _auditorRegistry;
        private static IWhisperAuditor _auditor;

        public static void Main(string[] args)
        {
            var parameters = GetParameters(args);

            StartAuditing(parameters.Username, parameters.Token);
            LoopApplication();
            EndAuditing();
        }

        private static Parameters GetParameters(string[] args)
        {
            try
            {
                return new Parameters(args);
            }
            catch (Exception)
            {
                System.Console.WriteLine("Usage: TwitchShoppingNetworkLogger.Console [username] [oauth token]");
                Environment.Exit(0);
                throw;
            }
        }

        private static void StartAuditing(string username, string token)
        {
            _userRepository = new UserRepository(ConfigManager.Instance, null);
            _auditorRegistry = new AuditorRegistry(_userRepository, ConfigManager.Instance);

            var excelFileManager = new ExcelFileManager(ConfigManager.Instance.ExcelDirectory);
            _auditor = _auditorRegistry.RegisterNewWhisperAuditor(username, token, new ExcelWhisperRepository(excelFileManager));
            _auditor.StartAuditing();
        }

        private static void LoopApplication()
        {
            bool loop = true;
            while (loop)
            {
                System.Console.WriteLine("Now logging whispers. Type 'quit' to end application.");
                string command = System.Console.ReadLine() ?? string.Empty;
                loop = !command.Trim().ToLower().Equals("quit");
            }
        }

        private static void EndAuditing()
        {
            _auditor.EndAuditing();
            System.Console.WriteLine("Client successfully disconnected.");
        }
    }
}
