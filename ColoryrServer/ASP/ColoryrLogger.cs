﻿using System.Collections.Concurrent;

namespace ColoryrServer.ASP
{
    internal class ColoryrLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
                            Exception? exception, Func<TState, Exception, string> formatter)
        {
            if (eventId.Id == 100 || eventId.Id == 101)
                return;
            if (logLevel is LogLevel.Warning or LogLevel.Error)
                ServerMain.LogError($"{logLevel}-{eventId.Id} {state} {exception} {exception.StackTrace}");
            else
                ServerMain.LogOut($"{logLevel}-{eventId.Id} {state} {exception}");
        }
    }

    class ColoryrLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ColoryrLogger> _loggers = new();

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new ColoryrLogger());
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
