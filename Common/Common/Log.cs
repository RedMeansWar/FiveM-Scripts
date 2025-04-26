using System;
using System.Diagnostics;
using Debug = CitizenFX.Core.Debug;

namespace Common
{
    public static class Log
    {
        /// <summary>
        /// Log Info with date and time.
        /// </summary>
        /// <param name="message">The message object that you want the message to say.</param>
        /// <param name="title">The custom title that you want the message to show, default is "RD INFO".</param>
        public static void InfoOrError(object message, string title = "RD INFO") 
            => Debug.WriteLine($"[{title ?? "RD INFO"} - {DateTime.Now:yyyyy/MM/dd HH:mm:ss}] {message ?? null}");

        /// <summary>
        /// Logs an error with readable information such as line number, and file name. (DateTime format follows Log.Info)
        /// </summary>
        /// <param name="message">The message object that you want the message to say.</param>
        /// <param name="ex">The exception to be thrown, ex is null by default.</param>
        public static void Error(object message, string title = "RD ERROR", Exception ex = null)
        {
            StackTrace trace = new(true);
            StackFrame frame = trace.GetFrame(0);
            int? lineNumber = frame?.GetFileLineNumber();
            string? fileName = frame?.GetFileName();

            Debug.WriteLine($"[{title ?? "RD ERROR"} - {DateTime.Now:yyyyy/MM/dd HH:mm:ss}] ERROR: {message}\nSOURCE: {fileName ?? "Unknown Source"}\nLINE: {lineNumber ?? 0}");
            if (ex is not null)
            {
                Debug.WriteLine($"[{title ?? "RD ERROR"}] - {DateTime.Now:yyyyy/MM/dd HH:mm:ss}] EXCEPTION ERROR: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }
    }
}