using CQRSlite.Messages;
using System;

namespace CQRSlite.Commands
{
    public interface ICommand : IMessage
    {
        Guid Id { get; set; }
        int ExpectedVersion { get; set; }
    }
}