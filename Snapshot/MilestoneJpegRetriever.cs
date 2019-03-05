using System;
using System.Threading;
using VideoOS.Platform;
using VideoOS.Platform.Live;

namespace Snapshot
{
    internal class MilestoneJpegRetriever
    {
        private readonly Item _item;
        private byte[] _bytes;
        private readonly ManualResetEventSlim _signal = new ManualResetEventSlim();

        public MilestoneJpegRetriever(Guid id)
        {
            VideoOS.Platform.SDK.Media.Environment.Initialize();
            _item = Configuration.Instance.GetItem(id, Kind.Camera);
        }

        public byte[] GetLiveJpeg()
        {
            try
            {
                var liveSource = new JPEGLiveSource(_item);
                liveSource.Init();
                liveSource.LiveContentEvent += LiveSourceOnLiveContentEvent;
                liveSource.LiveModeStart = true;
                var imageReceived = _signal.WaitHandle.WaitOne(TimeSpan.FromSeconds(10));
                liveSource.LiveModeStart = false;
                liveSource.Close();
                if (imageReceived)
                {
                    var buffer = new byte[_bytes.Length];
                    Array.Copy(_bytes, buffer, _bytes.Length);
                    _bytes = null;
                    return buffer;
                }
                else
                {
                    _bytes = null;
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving a live JPEG image. Error: {ex.Message}");
                return null;
            }
            finally
            {
                _signal.Reset();
            }

        }

        private void LiveSourceOnLiveContentEvent(object sender, EventArgs e)
        {
            var args = e as LiveContentEventArgs;
            if (args?.Exception != null)
                Console.WriteLine($"{args.Exception.Message}");
            if (args?.LiveContent?.Content != null)
            {
                _bytes = args.LiveContent.Content;
                _signal.Set();
            }
        }
    }
}