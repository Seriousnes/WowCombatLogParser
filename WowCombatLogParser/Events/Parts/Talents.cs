using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWCombatLogParser.Events.Parts
{
    public class Talents : EventSection
    {
        public int[] PvETalents { get; set; } = new int[6];
        public int[] PvPTalents { get; set; } = new int[4];

        public override void Parse(IEnumerator<string> enumerator)
        {            
        }
    }
}
