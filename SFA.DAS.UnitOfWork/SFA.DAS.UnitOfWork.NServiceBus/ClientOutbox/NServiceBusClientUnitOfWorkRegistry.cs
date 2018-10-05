﻿using StructureMap;

namespace SFA.DAS.UnitOfWork.NServiceBus.ClientOutbox
{
    public class NServiceBusClientUnitOfWorkRegistry : Registry
    {
        public NServiceBusClientUnitOfWorkRegistry()
        {
            IncludeRegistry<UnitOfWorkRegistry>();
            For<IUnitOfWork>().Add<UnitOfWork>();
            For<IUnitOfWorkManager>().Add<UnitOfWorkManager>();
        }
    }
}