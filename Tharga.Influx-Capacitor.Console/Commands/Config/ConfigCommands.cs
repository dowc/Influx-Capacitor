﻿using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.InfluxCapacitor.Console.Commands.Config
{
    internal class ConfigCommands : ContainerCommandBase
    {
        public ConfigCommands(ICompositeRoot compositeRoot)
            : base("Config")
        {
            RegisterCommand(new ConfigShowCommand(compositeRoot.InfluxDbAgentLoader, compositeRoot.ConfigBusiness));
            RegisterCommand(new ConfigAutoCommand(compositeRoot.InfluxDbAgentLoader, compositeRoot.ConfigBusiness, compositeRoot.CounterBusiness));
            RegisterCommand(new ConfigServerCommand(compositeRoot.InfluxDbAgentLoader, compositeRoot.ConfigBusiness));
            RegisterCommand(new ConfigDatabaseCommand(compositeRoot.InfluxDbAgentLoader, compositeRoot.ConfigBusiness));
            RegisterCommand(new ConfigureApplicationCommand(compositeRoot.InfluxDbAgentLoader, compositeRoot.ConfigBusiness));
            RegisterCommand(new ConfigListCommand(compositeRoot.ConfigBusiness));
        }
    }
}