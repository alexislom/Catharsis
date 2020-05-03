using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using ReactiveUI;

namespace CatharsisSampleForms.Helpers
{
    public class DefaultExceptionHandler : IObserver<Exception>
    {
        public void OnNext(Exception ex)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }

            RxApp.MainThreadScheduler.Schedule(_ => throw ex);
        }

        public void OnError(Exception ex)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }

            RxApp.MainThreadScheduler.Schedule(_ => throw ex);
        }

        public void OnCompleted()
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }

            RxApp.MainThreadScheduler.Schedule(_ => throw new NotImplementedException());
        }
    }
}