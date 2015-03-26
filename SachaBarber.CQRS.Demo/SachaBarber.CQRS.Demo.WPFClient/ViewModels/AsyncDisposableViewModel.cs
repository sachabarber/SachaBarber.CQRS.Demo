using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SachaBarber.CQRS.Demo.WPFClient.Controls;

namespace SachaBarber.CQRS.Demo.WPFClient.ViewModels
{
    public abstract class AsyncDisposableViewModel : DisposableViewModel
    {
        private string waitText;
        private string errorMessage;
        private AsyncType asyncState = AsyncType.Content;

        public string WaitText
        {
            get { return waitText; }
            set
            {
                RaiseAndSetIfChanged(ref waitText, value, () => WaitText);
            }
        }

        public AsyncType AsyncState
        {
            get { return asyncState; }
            set
            {
                RaiseAndSetIfChanged(ref asyncState, value, () => AsyncState);
            }
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                RaiseAndSetIfChanged(ref errorMessage, value, () => ErrorMessage);
            }
        }
    }
}
