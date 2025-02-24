using System;
using System.Diagnostics;
using System.Reflection;
using Tomori.TIAS.Security.WS.Helpers.Consts;

namespace Tomori.TIAS.Security.WS.Helpers
{
    public static class LogHelper
    {
        private static readonly string appName = AppConst.APP_NAME;
        private static readonly string source = AppConst.SOURCE;

        private static string HandleMessage(string message, string username = "")
        {
            string result = "";

            // Add username to message if exist
            if (username != "" && username != null)
            {
                result = $"USERNAME = {username}\n";
            }

            result = $"{result}MESSAGE = {message}\n";

            return result;
        }

        public static void SaveError(string message, string username = "")
        {
            try
            {
                if (!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, appName);
                }
                using (EventLog eventLog = new EventLog(appName))
                {
                    eventLog.Source = source;
                    eventLog.WriteEntry(HandleMessage(message, username), EventLogEntryType.Error);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error: {ex.Message}");
            }
        }

        public static void SaveError(MethodBase method, string message, string username = "")
        {
            try
            {
                if (!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, appName);
                }
                using (EventLog eventLog = new EventLog(appName))
                {
                    eventLog.Source = source;

                    eventLog.WriteEntry(HandleMessage($"{GetInfoMethod(method)}: {message}", username), EventLogEntryType.Error);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error: {ex.Message}");
            }
        }

        public static void SaveInformation(string message, string username)
        {
            try
            {
                if (!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, appName);
                }
                using (EventLog eventLog = new EventLog(appName))
                {
                    eventLog.Source = source;
                    eventLog.WriteEntry(HandleMessage(message, username), EventLogEntryType.Information);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error: {ex.Message}");
            }
        }

        public static string GetInfoMethod(MethodBase method)
        {
            if (method == null)
                return "Error LogHelper GetInfoMethod, method is null";
            
            ParameterInfo[] parameters = method.GetParameters();
            string parameterDetails = string.Join(", ", Array.ConvertAll(parameters, p => $"{p.ParameterType.Name} {p.Name}"));
            return $"Error in Class {method.DeclaringType.Name} function {method.Name}<{((MethodInfo)method).ReturnType.Name}>({parameterDetails})";
        }
    }
}