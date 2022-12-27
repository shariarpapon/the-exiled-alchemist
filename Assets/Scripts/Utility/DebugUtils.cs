using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.DebuggingUtility
{
    public class DebugUtils
    {
        public static void LogPositive(object msg) => Print(msg, "#00FF6F");
        public static void LogNegative(object msg) => Print(msg, "#FC5C5C");
        public static void LogCheck(object msg) => Print(msg, "#FFD96E");
        public static void LogConfirmation(object msg) => Print(msg, "#A0B9FF");
        public static void LogFailed(object msg) => Print(msg, "#F7A278");

        public static void Print(object msg, string hex) => 
            Debug.Log($"<color={hex}>{msg}</color>");
    }
}
