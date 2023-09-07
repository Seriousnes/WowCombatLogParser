using System;
using System.Collections.Generic;
using System.Reflection;

namespace WoWCombatLogParser.Common.Utility;

public class ClassMap
{
    public ObjectActivator Constructor { get; set; }
    public IList<PropertyInfo> Properties { get; set; }
    public List<Attribute> CustomAttributes { get; set; }
}
