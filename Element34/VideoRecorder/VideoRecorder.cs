using Microsoft.Extensions.Logging;
using SharpAvi;
using SharpAvi.Codecs;
using SharpAvi.Output;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Element34.VideoRecorder
{
    /// <summary>
    /// Parameters for configuring the screen recorder.
    /// </summary>
    public class RecorderParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecorderParams"/> class.
        /// </summary>
        /// <param name="filename">The output filename for the recording.</param>
        /// <param name="contextName">The context name for the recording.</param>
        /// <param name="frameRate">The frame rate of the recording.</param>
        /// <param name="encoder">The codec to use for encoding the video.</param>
        /// <param name="quality">The quality of the recording.</param>
        /// <exception cref="ArgumentException">Thrown when filename or contextName is null or empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when frameRate is less than or equal to 0, or quality is not between 0 and 100.</exception>
        public RecorderParams(string filename, string contextName, int frameRate, FourCC encoder, int quality)
        {
            if (string.IsNullOrEmpty(filename)) throw new ArgumentException("Filename cannot be null or empty", nameof(filename));
            if (string.IsNullOrEmpty(contextName)) throw new ArgumentException("ContextName cannot be null or empty", nameof(contextName));
            if (frameRate <= 0) throw new ArgumentOutOfRangeException(nameof(frameRate), "FrameRate must be greater than zero");
            if (quality < 0 || quality > 100) throw new ArgumentOutOfRangeException(nameof(quality), "Quality must be between 0 and 100");

            FileName = filename;
            ContextName = contextName;
            FramesPerSecond = frameRate;
            Codec = encoder;
            Quality = quality;

            Height = Screen.PrimaryScreen.Bounds.Height;
            Width = Screen.PrimaryScreen.Bounds.Width;
        }

        readonly string FileName;
        public string ContextName { get; private set; }
        public int FramesPerSecond, Quality;
        static FourCC Codec;

        public int Height { get; private set; }
        public int Width { get; private set; }

        /// <summary>
        /// Creates an AviWriter instance with the specified parameters.
        /// </summary>
        /// <returns>A configured <see cref="AviWriter"/> instance.</returns>
        public AviWriter CreateAviWriter()
        {
            return new AviWriter(FileName)
            {
                FramesPerSecond = FramesPerSecond,
                EmitIndex1 = true,
            };
        }

        /// <summary>
        /// Creates a video stream with the specified parameters.
        /// </summary>
        /// <param name="writer">The AviWriter instance to which the video stream will be added.</param>
        /// <returns>A configured <see cref="IAviVideoStream"/> instance.</returns>
        public IAviVideoStream CreateVideoStream(AviWriter writer)
        {
            if (Codec == CodecIds.Uncompressed)
                return writer.AddUncompressedVideoStream(Width, Height);

            if (Codec == CodecIds.MotionJpeg)
                return writer.AddMJpegWpfVideoStream(Width, Height, Quality);

            return writer.AddMpeg4VcmVideoStream(Width, Height, (double)writer.FramesPerSecond,
                quality: Quality,
                codec: Codec,
                forceSingleThreadedAccess: true);
        }
    }

    /// <summary>
    /// A class to handle screen recording functionality.
    /// </summary>
    public class Recorder : IDisposable
    {
        private readonly AviWriter writer;
        private readonly RecorderParams Params;
        private readonly IAviVideoStream videoStream;
        private readonly Thread screenThread;
        private readonly ManualResetEvent stopThread = new ManualResetEvent(false);
        private readonly bool record;
        private readonly ILogger<Recorder> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Recorder"/> class.
        /// </summary>
        /// <param name="Params">The parameters for configuring the recorder.</param>
        /// <param name="record">A flag to control additional behavior in the recorder.</param>
        /// <param name="logger">An optional logger instance for logging purposes.</param>
        /// <exception cref="ArgumentNullException">Thrown when Params is null.</exception>
        public Recorder(RecorderParams Params, bool record, ILogger<Recorder> logger = null)
        {
            this.Params = Params ?? throw new ArgumentNullException(nameof(Params));
            this.record = record;
            this.logger = logger;

            if (record)
            {
                logger?.LogInformation("Recorder enabled");

                try
                {
                    writer = Params.CreateAviWriter();
                    videoStream = Params.CreateVideoStream(writer);
                    videoStream.Name = Params.ContextName;

                    screenThread = new Thread(RecordScreen)
                    {
                        Name = typeof(Recorder).Name + ".RecordScreen",
                        IsBackground = true
                    };

                    screenThread.Start();
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "Error initializing Recorder");
                    Dispose();
                    throw;
                }
            }
            else
            {
                logger?.LogInformation("Recorder disabled");
            }
        }

        /// <summary>
        /// Disposes the resources used by the Recorder.
        /// </summary>
        public void Dispose()
        {
            stopThread.Set();
            screenThread?.Join();
            writer?.Close();
            stopThread.Dispose();
        }

        /// <summary>
        /// Records the screen based on the specified parameters.
        /// </summary>
        private void RecordScreen()
        {
            TimeSpan frameInterval = TimeSpan.FromSeconds(1 / (double)writer.FramesPerSecond);
            byte[] buffer = new byte[Params.Width * Params.Height * 4];
            Task videoWriteTask = null;
            TimeSpan timeTillNextFrame = TimeSpan.Zero;

            while (!stopThread.WaitOne(timeTillNextFrame))
            {
                var timestamp = DateTime.Now;

                try
                {
                    CaptureScreenshot(buffer);
                    videoWriteTask?.Wait();
                    videoWriteTask = videoStream.WriteFrameAsync(true, buffer, 0, buffer.Length);
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "Error during screen recording");
                }

                timeTillNextFrame = timestamp + frameInterval - DateTime.Now;
                if (timeTillNextFrame < TimeSpan.Zero)
                    timeTillNextFrame = TimeSpan.Zero;
            }

            videoWriteTask?.Wait();
        }

        /// <summary>
        /// Captures a screenshot of the screen and stores it in the provided buffer.
        /// </summary>
        /// <param name="buffer">The buffer to store the screenshot data.</param>
        private void CaptureScreenshot(byte[] buffer)
        {
            using (Bitmap bmp = new Bitmap(Params.Width, Params.Height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, new Size(Params.Width, Params.Height), CopyPixelOperation.SourceCopy);
                    g.Flush();

                    var bits = bmp.LockBits(new Rectangle(0, 0, Params.Width, Params.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                    Marshal.Copy(bits.Scan0, buffer, 0, buffer.Length);
                    bmp.UnlockBits(bits);
                }
            }
        }
    }
}
