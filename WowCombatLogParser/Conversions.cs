using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWCombatLogParser
{
    public static class Conversions
    {
        public static T GetValue<T>(string value)
        {
            object convertableValue;
            
            switch(typeof(T))
            {
                case var date when date == typeof(DateTime):
                    convertableValue = DateTime.ParseExact(value, "M/d HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    break;
                case var hex when hex == typeof(long):
                    convertableValue = Convert.ToInt32(value, value.StartsWith("0x") ? 16 : 10);                    
                    break;
                case var logical when logical == typeof(bool):
                    convertableValue = (value == "-1");
                    break;
                default:
                    convertableValue = value;
                    break;
            }

            return (T)Convert.ChangeType(convertableValue, typeof(T));
        }
    }
}
