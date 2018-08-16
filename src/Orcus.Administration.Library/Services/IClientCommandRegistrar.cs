using System;

namespace Orcus.Administration.Library.Services
{
    public interface IClientCommandRegistrar
    {
        void RegisterView(Type viewType, string txLibResource, object icon, CommandCategory category);
    }

    public enum CommandCategory
    {
        Fun,
        Interaction
    }
}