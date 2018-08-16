using System;

namespace UserInteraction.Dtos
{
    [Serializable]
    public enum SystemIcon : byte
    {
        Error,
        Info,
        Warning,
        Question,
        None
    }
}