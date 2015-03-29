//using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SachaBarber.CQRS.Demo.WPFClient.ViewModels
{
    public abstract class DialogViewModel : INPCBase, IDialogViewModel
    {
        private string title;
        private bool isClosable;
        private bool? result;
        private bool close;


        protected DialogViewModel(string title)
        {
            Title = title;
            IsClosable = true;
        }

        public string Title
        {
            get
            {
                return this.title;
            }
            protected set
            {
                RaiseAndSetIfChanged(ref this.title, value, () => Title);
            }
        }


        public bool IsClosable
        {
            get
            {
                return this.isClosable;
            }
            protected set
            {
                RaiseAndSetIfChanged(ref this.isClosable, value, () => IsClosable);
            }
        }

        public bool? Result
        {
            get
            {
                return this.result;
            }
            private set
            {
                RaiseAndSetIfChanged(ref this.result, value, () => Result);
            }
        }

        public bool Close
        {
            get
            {
                return this.close;
            }
            private set
            {
                RaiseAndSetIfChanged(ref this.close, value, () => Close);
            }
        }

        protected void CloseDialog(bool? result)
        {
            Result = result;
            Close = true;
        }
    }
}

