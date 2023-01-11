using Godot;
using System;
using System.Collections.Generic;

namespace Mars_Seal_Crimson
{
    public static class InputAssistance
    {
        public static Dictionary<string, DateTime> keyBounceMap = new Dictionary<string, DateTime>();
        public static Dictionary<string, DateTime> keyActivateMap = new Dictionary<string, DateTime>();
        public static Dictionary<string, Int32> keyBounceMap_Ticks = new Dictionary<string, Int32>();
        public static Dictionary<string, Int32> keyActivateMap_Ticks = new Dictionary<string, Int32>();


        public static bool verboseFlag = false;

        public static void SetVerbose(bool verbose)
        {
            verboseFlag = verbose;
        }

        public static bool KeyBounceCheckAlternative(string key, float bounceIgnore, float skipSeconds)
        {
            bool res = true;
            if ((key != null) && (keyActivateMap_Ticks.ContainsKey(key)))
            {
                Int32 keyTimestamp = keyActivateMap_Ticks[key];
                Int32 currTicks = System.Environment.TickCount;

                var diff = currTicks - keyTimestamp;
                if (diff < (Int32)(skipSeconds * 1000.0f))
                {
                    res = false;
                    return res;
                }
                else
                {
                    //GD.Print($"KeyBounceCheck, skip triggered, key = {key} timestamp = {keyTimestamp}");
                    keyBounceMap_Ticks.Remove(key);
                    keyActivateMap_Ticks.Remove(key);
                    keyActivateMap_Ticks.Add(key, currTicks);
                    return res;
                }
            }
            if ((key != null) && (bounceIgnore >= 0.1))
            {
                Int32 currTicks = System.Environment.TickCount;
                if (keyBounceMap_Ticks.ContainsKey(key))
                {
                    Int32 keyTimestamp = keyBounceMap_Ticks[key];
                    var diff = currTicks - keyTimestamp;

                    if (diff >= (Int32)(bounceIgnore * 1000.0f))
                    {
                        if (diff >= (Int32)(2 * bounceIgnore * 1000.0f))
                            keyBounceMap.Remove(key);
                        res = false;
                    }
                }
                else
                {
                    keyBounceMap_Ticks.Add(key, currTicks);
                    keyActivateMap_Ticks.Add(key, currTicks);
                }
            }
            return res;
        }

    }

}
