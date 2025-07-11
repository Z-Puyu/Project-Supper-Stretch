using Project.Scripts.Audio;

namespace Project.Scripts.UI.Control;

public class UserInterfaceAudio : AudioPlayer<UserInterfaceAudio.Sound> {
    public enum Sound {
        Enable,
        Disable,
        Interacted,
        Hovered
    }
}
