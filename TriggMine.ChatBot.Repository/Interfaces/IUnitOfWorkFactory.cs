using System;
using System.Collections.Generic;
using System.Text;

namespace TriggMine.ChatBot.Repository.Interfaces
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create();
    }
}
