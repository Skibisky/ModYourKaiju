using System;
using UnityEngine;

public static class BepLog
{

    public static EventHandler<string> OnLog;

    public static void Log(string message)
    {
        if (OnLog != null)
            OnLog.Invoke(null, message);
        else
            Debug.Log(message);
    }
}
