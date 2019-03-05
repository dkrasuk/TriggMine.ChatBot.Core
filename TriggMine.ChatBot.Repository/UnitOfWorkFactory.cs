using System;
using System.Collections.Generic;
using System.Text;
using TriggMine.ChatBot.Repository.Interfaces;

namespace TriggMine.ChatBot.Repository
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IUnitOfWork Create() => new UnitOfWork();
    }
}
