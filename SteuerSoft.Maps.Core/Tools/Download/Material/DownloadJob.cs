using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteuerSoft.Maps.Core.Tools.Download.Material
{
   /// <summary>
   /// A delegate for functions that are called when the download job has finished.
   /// </summary>
   /// <param name="sender">The sender, usually a DownloadService.</param>
   /// <param name="info">The info object passed with the download job.</param>
   /// <param name="stream">A Stream to the data downloaded.</param>
   public delegate void DownloadFinishedDelegate(object sender, object info, MemoryStream stream);

   /// <summary>
   /// A delegate for functions that are called when the download of a job has failed.
   /// </summary>
   /// <param name="sender">The sender, usually a DownloadService.</param>
   /// <param name="info">The info object passed with the download job.</param>
   public delegate void DownloadFailedDelegate(object sender, object info);

   /// <summary>
   /// A job that will be added to the downloadservice.
   /// </summary>
   public class DownloadJob
   {
      /// <summary>
      /// The URL that shall be downloaded.
      /// </summary>
      public string Url { get; set; }

      /// <summary>
      /// An object containing information about the data being downloaded.
      /// </summary>
      public object Info { get; set; }

      /// <summary>
      /// Event that is thrown when the download of the job finished.
      /// </summary>
      public event DownloadFinishedDelegate OnFinished;

      /// <summary>
      /// Event that is thrown when the download of the job failed.
      /// </summary>
      public event DownloadFailedDelegate OnFailed;

      /// <summary>
      /// Creates a new instance of DownloadJob.
      /// </summary>
      public DownloadJob()
         : this(string.Empty, null)
      {
         
      }

      /// <summary>
      /// Creates a new DownloadJob with a specific URL to download and an info object.
      /// </summary>
      /// <param name="url">The URL that shall be downloaded.</param>
      /// <param name="info">The info object to be stored with the job.</param>
      public DownloadJob(string url, object info)
      {
         Url = url;
         Info = info;
      }

      /// <summary>
      /// Sends the finished events to all registered event handlers.
      /// </summary>
      /// <param name="dataStream">The data stream that shall be passed to all event handlers.</param>
      public void SendFinishedEvent(MemoryStream dataStream)
      {
         OnFinished?.Invoke(this, Info, dataStream);
      }

      /// <summary>
      /// Sends the failed event to all registered event handlers.
      /// </summary>
      public void SendFailedEvent()
      {
         OnFailed?.Invoke(this, Info);
      }
   }
}
