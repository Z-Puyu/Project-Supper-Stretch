using UnityEngine;

namespace Project.Scripts.Common;

public static class Logging {
    public static void Info(string message, Object context) {
        Debug.Log(message, context);   
    }
    
    public static void Error(string message, Object context) {
        Debug.LogError(message, context);   
    }
    
    public static void Warn(string message, Object context) {
        Debug.LogWarning(message, context);   
    }

    public static void Assert(bool condition, string message, Object context) {
        Debug.Assert(condition, message, context);  
    }
    
    public static void Assert(bool condition, string message) {
        Debug.Assert(condition, message);  
    }
    
    public static void Assert(bool condition) {
        Debug.Assert(condition);  
    }
}
