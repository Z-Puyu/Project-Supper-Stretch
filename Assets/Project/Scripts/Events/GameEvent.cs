using UnityEngine;

namespace Project.Scripts.Events;

public record class GameEvent<T>(Object Sender, T Data);
