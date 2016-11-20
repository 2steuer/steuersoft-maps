using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using SteuerSoft.Maps.Core.Tools.Download.Material;

namespace SteuerSoft.Maps.Core.Tools.Download
{
   /// <summary>
   /// This class offers functionality to download files.
   /// Since this is threaded, we can specify a number of threads that is paralell downloading data.
   /// </summary>
   public class DownloadService
   {
      /// <summary>
      /// The queue of jobs that shall be downloaded.
      /// </summary>
      private ConcurrentQueue<DownloadJob> _jobs = new ConcurrentQueue<DownloadJob>();

      /// <summary>
      /// Mutex for the current thread counter.
      /// </summary>
      Mutex _threadCounterMutex = new Mutex();

      /// <summary>
      /// Number of currently running threads.
      /// </summary>
      private int _currentThreads = 0;

      /// <summary>
      /// Number of threads that may be running at the same time.
      /// </summary>
      public int MaxThreads { get; set; } = 2;

      /// <summary>
      /// The HTTP User Agent that shall be sent to the webserver.
      /// </summary>
      public string UserAgent { get; set; }

      /// <summary>
      /// Enqueues a job to the job queue. If there are more jobs than started threads,
      /// new threads will be started up to a number of MaxThreads threads.
      /// </summary>
      /// <param name="job"></param>
      public void AddJob(DownloadJob job)
      {
         _jobs.Enqueue(job);

         _threadCounterMutex.WaitOne();
         if (_currentThreads < _jobs.Count)
         {
            for (int i = 0; i < (_jobs.Count - _currentThreads) && _currentThreads < MaxThreads; i++)
            {
               Thread t = new Thread(DownloadThread);
               t.Start();
               _currentThreads++;
            }
         }
         _threadCounterMutex.ReleaseMutex();
      }

      /// <summary>
      /// The thread handling the download of the data.
      /// </summary>
      private void DownloadThread()
      {
         while (_jobs.Count > 0)
         {
            DownloadJob currentJob;
            if (_jobs.TryDequeue(out currentJob))
            {
               using (WebClient client = new WebClient())
               {
                  client.Headers.Add("User-Agent", UserAgent);

                  MemoryStream memStr;

                  try
                  {
                     memStr = new MemoryStream(client.DownloadData(currentJob.Url));
                     currentJob.SendFinishedEvent(memStr);
                  }
                  catch (Exception ex)
                  {
                     currentJob.SendFailedEvent();
                  }
               }
            }
         }

         _threadCounterMutex.WaitOne();
         _currentThreads--;
         _threadCounterMutex.ReleaseMutex();
      }
   }
}
