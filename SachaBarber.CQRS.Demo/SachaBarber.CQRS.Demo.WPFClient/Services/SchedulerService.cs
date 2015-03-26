using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

namespace SachaBarber.CQRS.Demo.WPFClient.Services
{
    public class SchedulerService : ISchedulerService
    {
        public IScheduler Immediate
        {
            get { return Scheduler.Immediate; }
        }

        public IScheduler CurrentThread
        {
            get { return Scheduler.CurrentThread; }
        }

        public IScheduler NewThread
        {
            get { return NewThreadScheduler.Default; }
        }

        public IScheduler ThreadPool
        {
            get { return ThreadPoolScheduler.Instance; }
        }

        public IScheduler TaskPool
        {
            get { return TaskPoolScheduler.Default; }
        }

        public IScheduler Dispatcher
        {
            
            get { return DispatcherScheduler.Current; }
        }

        public IScheduler EventLoop
        {
            get { return new EventLoopScheduler(); }
        }
    }
}
