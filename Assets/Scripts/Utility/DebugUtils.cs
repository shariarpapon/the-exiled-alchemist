using UnityEngine;

namespace Everime.DebuggingUtility
{
    /// <summary>
    /// This class contains various debugging methods.
    /// </summary>
    public static class DebugUtils
    {
        public static void LogPositive(object msg) => Log(msg, "#00FF6F");
        public static void LogNegative(object msg) => Log(msg, "#FC5C5C");
        public static void LogCheck(object msg) => Log(msg, "#FFD96E");
        public static void LogConfirmation(object msg) => Log(msg, "#A0B9FF");
        public static void LogFailed(object msg) => Log(msg, "#F7A278");

        public static void LimitedLog(object msg, int limit, ref int counter) 
        {
            if (counter >= limit) return;

            counter++;
            LogCheck(msg);
        }

        public static void Log(object msg, string hex) => 
            Debug.Log($"<color={hex}>{msg}</color>");
    }
}
