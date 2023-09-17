using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Parser;

namespace WoWCombatLogParser.Models;

public interface IParsable
{
    Task Parse(IList<ICombatLogDataField> data);
    Task Parse(string line);
    IApplicationContext ApplicationContext { get; set; }
}
