using System;
using UnityEngine;

namespace Project.Scripts.Common;

public static class GameEvents {
    public static Action OnPause { get; set; } = delegate { };
    public static Action OnPlay { get; set; } = delegate { };
    public static Action<GameNotification, GameObject> OnNotification { get; set; } = delegate { };

    public static class UI {
        public static Action OnGoBack = delegate { };
    }
}
