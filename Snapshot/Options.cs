using System;
using CommandLine;

namespace Snapshot
{
    internal class Options
    {
        [Option('s', "server", Required = false, Default = "localhost", HelpText = "Server hostname or IP for the Management Server")]
        public string Host { get; set; }

        [Option('p', "port", Required = false, Default = 80, HelpText = "HTTP port for the Management Server")]
        public int Port { get; set; }

        [Option('c', "camera", Required = true, HelpText = "Camera ID/GUID")]
        public Guid CameraId { get; set; }

        [Option('o', "output", Required = false, Default = ".\\", HelpText = "Output path")]
        public string Path { get; set; }

    }
}