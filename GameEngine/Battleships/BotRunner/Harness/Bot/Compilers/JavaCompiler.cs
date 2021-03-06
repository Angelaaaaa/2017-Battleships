﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BotRunner.Properties;
using BotRunner.Util;
using Domain.Bot;
using Domain.Meta;
using GameEngine.Loggers;

namespace TestHarness.TestHarnesses.Bot.Compilers
{
    public class JavaCompiler : ICompiler
    {
        private readonly BotMeta _botMeta;
        private readonly string _botDir;
        private readonly ILogger _compileLogger;
        private readonly EnvironmentSettings _environmentSettings;

        public JavaCompiler(BotMeta botMeta, string botDir, ILogger compileLogger, EnvironmentSettings environmentSettings)
        {
            _botMeta = botMeta;
            _botDir = botDir;
            _compileLogger = compileLogger;
            _environmentSettings = environmentSettings;
        }

        public bool HasPackageManager()
        {
            return true;
        }

        public bool RunPackageManager()
        {
            return true;
        }

        public bool RunCompiler()
        {
            var compileLocation = Path.Combine(_botDir, _botMeta.ProjectLocation ?? "");
            var path = Path.Combine(compileLocation, "pom.xml");
            var exists = File.Exists(path);

            _compileLogger.LogInfo("Checking if bot " + _botMeta.NickName + " has a pom file at " + compileLocation);
            if (exists)
            {
                _compileLogger.LogInfo("Compiling bot " + _botMeta.NickName + " using maven");
                using (var handler = new ProcessHandler(compileLocation, _environmentSettings.PathToMaven, "clean compile package assembly:single -e", _compileLogger))
                {
                    handler.ProcessToRun.ErrorDataReceived += ProcessDataRecieved;
                    handler.ProcessToRun.OutputDataReceived += ProcessDataRecieved;

                    return handler.RunProcess() == 0;
                }
            }
            _compileLogger.LogInfo("Java projects need to use maven in order to compile their projects");

            return false;
        }

        void ProcessDataRecieved(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            _compileLogger.LogInfo(e.Data);
        }
    }
}
