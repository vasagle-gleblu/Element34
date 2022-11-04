using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpAvi;
using SharpAvi.Codecs;
using SharpAvi.Output;

namespace Element34.Utilities.VideoRecorder
{
    public class RecorderParams
    {   // CSharp-screen-recorder by https://github.com/vmille25
        // Updated for SharpAvi v3.x.x

        public RecorderParams(string filename, string contextName, int FrameRate, FourCC Encoder, int Quality)
        {
            FileName = filename;
            ContextName = contextName;
            FramesPerSecond = FrameRate;
            Codec = Encoder;
            this.Quality = Quality;

            Height = Screen.PrimaryScreen.Bounds.Height;
            Width = Screen.PrimaryScreen.Bounds.Width;
        }

        readonly string FileName;
        public string ContextName { get; private set; }
        public int FramesPerSecond, Quality;
        static FourCC Codec;

        public int Height { get; private set; }
        public int Width { get; private set; }

        public AviWriter CreateAviWriter()
        {
            return new AviWriter(FileName)
            {
                FramesPerSecond = FramesPerSecond,
                EmitIndex1 = true,
            };
        }

        public IAviVideoStream CreateVideoStream(AviWriter writer)
        {
            // Select encoder type based on FOURCC of codec
            if (Codec == CodecIds.Uncompressed)
                return writer.AddUncompressedVideoStream(Width, Height);

            else if (Codec == CodecIds.MotionJpeg)
                return writer.AddMJpegWpfVideoStream(Width, Height, Quality);

            else
                return writer.AddMpeg4VcmVideoStream(Width, Height, (double)writer.FramesPerSecond,
                    // It seems that all tested MPEG-4 VfW codecs ignore the quality affecting parameters passed through VfW API
                    // They only respect the settings from their own configuration dialogs, and Mpeg4VideoEncoder currently has no support for this
                    quality: Quality,
                    codec: Codec,
                    // Most of VfW codecs expect single-threaded use, so we wrap this encoder to special wrapper
                    // Thus all calls to the encoder (including its instantiation) will be invoked on a single thread although encoding (and writing) is performed asynchronously
                    forceSingleThreadedAccess: true);
        }
    }

    public class Recorder : IDisposable
    {
        #region Fields
        readonly AviWriter writer;
        readonly RecorderParams Params;
        readonly IAviVideoStream videoStream;
        readonly Thread screenThread;
        readonly ManualResetEvent stopThread = new ManualResetEvent(false);
        #endregion

        public Recorder(RecorderParams Params)
        {
            this.Params = Params;

            // Create AVI writer and specify FPS
            writer = Params.CreateAviWriter();

            // Create video stream
            videoStream = Params.CreateVideoStream(writer);

            // Set only name. Other properties were when creating stream, 
            // either explicitly by arguments or implicitly by the encoder used
            videoStream.Name = Params.ContextName;

            screenThread = new Thread(RecordScreen)
            {
                Name = typeof(Recorder).Name + ".RecordScreen",
                IsBackground = true
            };

            screenThread.Start();
        }

        public void Dispose()
        {
            stopThread.Set();
            screenThread.Join();

            // Close writer: the remaining data is written to a file and file is closed
            writer.Close();

            stopThread.Dispose();
        }

        void RecordScreen()
        {
            TimeSpan frameInterval = TimeSpan.FromSeconds(1 / (double)writer.FramesPerSecond);
            byte[] buffer = new byte[Params.Width * Params.Height * 4];
            Task videoWriteTask = null;
            TimeSpan timeTillNextFrame = TimeSpan.Zero;

            while (!stopThread.WaitOne(timeTillNextFrame))
            {
                var timestamp = DateTime.Now;

                Screenshot(buffer);

                // Wait for the previous frame is written
                videoWriteTask?.Wait();

                // Start asynchronous (encoding and) writing of the new frame
                videoWriteTask = videoStream.WriteFrameAsync(true, buffer, 0, buffer.Length);

                timeTillNextFrame = timestamp + frameInterval - DateTime.Now;
                if (timeTillNextFrame < TimeSpan.Zero)
                    timeTillNextFrame = TimeSpan.Zero;
            }

            // Wait for the last frame is written
            videoWriteTask?.Wait();
        }

        public void Screenshot(byte[] Buffer)
        {
            using (Bitmap BMP = new Bitmap(Params.Width, Params.Height))
            {
                using (Graphics newGraphics = Graphics.FromImage(BMP))
                {
                    newGraphics.CopyFromScreen(Point.Empty, Point.Empty, new Size(Params.Width, Params.Height), CopyPixelOperation.SourceCopy);

                    newGraphics.Flush();

                    var bits = BMP.LockBits(new Rectangle(0, 0, Params.Width, Params.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                    Marshal.Copy(bits.Scan0, Buffer, 0, Buffer.Length);
                    BMP.UnlockBits(bits);
                }
            }
        }
    }
}

/*
    USAGE:

    var rec = new Recorder(new RecorderParams("out.avi", 10, SharpAvi.KnownFourCCs.Codecs.MotionJpeg, 70));

    Console.WriteLine("Press any key to Stop...");
    Console.ReadKey();

    // Finish Writing
    rec.Dispose();

    // Or...
    using (var rec = new Recorder(new RecorderParams("out.avi", "out", 10, CodecIds.MotionJpeg, 70)))
    {
        ...
    }
 
 */
