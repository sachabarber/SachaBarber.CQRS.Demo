using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace SachaBarber.CQRS.Demo.WPFClient.ViewModels
{
    public abstract class DisposableViewModel : INPCBase, IDisposable
    {
        private CompositeDisposable disposables = new CompositeDisposable();

        public void AddDisposable(IDisposable disposable)
        {
            disposables.Add(disposable);
        }

        public virtual void Dispose()
        {
            disposables.Dispose();
        }
    }
}
