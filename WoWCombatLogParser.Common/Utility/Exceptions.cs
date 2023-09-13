using System;
using System.Reflection;
using System.Runtime.Serialization;
using WoWCombatLogParser.Common.Events;

namespace WoWCombatLogParser.Common.Utility;

public class WowCombatlogParserException : Exception
{
    public WowCombatlogParserException()
    {
    }

    public WowCombatlogParserException(string message) : base(message)
    {
    }

    public WowCombatlogParserException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected WowCombatlogParserException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

public class WowCombatlogParserPropertyException : WowCombatlogParserException
{
    public WowCombatlogParserPropertyException()
    {
    }

    public WowCombatlogParserPropertyException(string message) : base(message)
    {
    }

    public WowCombatlogParserPropertyException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public WowCombatlogParserPropertyException(PropertyInfo property, ICombatLogEventComponent @CombatLogEventComponent, string message, Exception innerException = null) : base(message, innerException)
    {
        Property = property;
        CombatLogEventComponent = @CombatLogEventComponent;
    }

    protected WowCombatlogParserPropertyException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public PropertyInfo Property { get; }
    public ICombatLogEventComponent CombatLogEventComponent { get; }
}
