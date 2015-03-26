using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SachaBarber.CQRS.Demo.WPFClient.ViewModels
{
    public interface IDialogViewModel
    {
        string Title
        {
            get;
        }

        bool? Result
        {
            get;
        }

        bool Close
        {
            get;
        }

        bool IsClosable
        {
            get;
        }
    }
}
