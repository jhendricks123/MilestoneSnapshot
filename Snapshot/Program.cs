using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using MipSdkHelper;
using VideoOS.Platform.Live;

namespace Snapshot
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptionsAndReturnExitCode);

        }

        private static void RunOptionsAndReturnExitCode(Options opts)
        {
            using (var client = new MipSdkClient(new Uri($"http://{opts.Host}:{opts.Port}")))
            {
                Console.WriteLine($"Logging in. . .");
                var loginResult = client.Login();
                Console.WriteLine(loginResult.Message);
                if (!loginResult.Success) return;

                var jpegRetriever = new MilestoneJpegRetriever(opts.CameraId);
                Console.WriteLine($"Connecting to live stream. . .");
                var bytes = jpegRetriever.GetLiveJpeg();
                if (bytes != null)
                {
                    Console.WriteLine($"Writing image to disk. . .");
                    File.WriteAllBytes(Path.Combine(opts.Path, $"{opts.CameraId}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.jpg"), bytes);
                    Console.WriteLine("Done.");
                }
                else
                {
                    Console.WriteLine("Failed to receive a live image.");
                }
            }
        }
    }
}
