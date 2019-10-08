using Fiddler;
using FiddlerCoreWpfDemo.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FiddlerCoreWpfDemo.Infrastructure
{
    internal class SessionsPersister
    {
        private readonly ICollection<Session> sessions = new List<Session>();
        private readonly ReaderWriterLockSlim sessionsLock = new ReaderWriterLockSlim();

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly string sazFilesDir = Path.Combine(Common.AssemblyDir, "..", "..", "SazFiles");
        private readonly string sazFilesPassword = string.Empty;

        private readonly TimeSpan interval = new TimeSpan(hours: 0, minutes: 0, seconds: 10);
        private readonly IDateTimeProvider dateTime;

        public SessionsPersister(IDateTimeProvider dateTime)
        {
            this.dateTime = dateTime;

            Directory.CreateDirectory(this.sazFilesDir);

            Task t = this.PeriodicPersistAsync(this.cancellationTokenSource.Token);
        }

        public void AddSession(Session session)
        {
            try
            {
                this.sessionsLock.EnterWriteLock();
                sessions.Add(session);
            }
            finally
            {
                this.sessionsLock.ExitWriteLock();
            }
        }

        public void Cancel()
        {
            this.cancellationTokenSource.Cancel();
        }

        public async Task PersistSessionsAsync(bool onlyCompleted = true)
        {
            string filename = $"{this.sazFilesDir}{Path.DirectorySeparatorChar}{this.dateTime.Now.ToString("hh-mm-ss")}.saz";

            await Task.Run(() =>
            {
                IEnumerable<Session> sessionsToPersist;

                try
                {
                    this.sessionsLock.EnterReadLock();
                    sessionsToPersist = this.sessions.Where(s => onlyCompleted ? s.state >= SessionStates.Done : true).ToList();
                }
                finally
                {
                    this.sessionsLock.ExitReadLock();
                }

                if (sessionsToPersist != null && sessionsToPersist.Any())
                {
                    bool success = Utilities.WriteSessionArchive(filename, sessionsToPersist.ToArray(), sazFilesPassword, false);
                    if (success)
                    {
                        this.ClearSessions(sessionsToPersist);
                    }
                }

            }).ConfigureAwait(false);
        }

        private void ClearSessions(IEnumerable<Session> persistedSessions)
        {
            try
            {
                this.sessionsLock.EnterWriteLock();
                foreach (Session session in persistedSessions)
                {
                    this.sessions.Remove(session);
                }
            }
            finally
            {
                this.sessionsLock.ExitWriteLock();
            }
        }

        private async Task PeriodicPersistAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                await Task.Delay(interval, cancellationToken).ConfigureAwait(false);
                await this.PersistSessionsAsync().ConfigureAwait(false);
            }
        }
    }
}
