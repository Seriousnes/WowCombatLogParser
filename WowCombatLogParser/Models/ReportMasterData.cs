using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWCombatLogParser.Models;
internal class ReportMasterData
{
    public List<Unit> Actors { get; set; }
    public List<Ability> Abilities { get; set; }
}
