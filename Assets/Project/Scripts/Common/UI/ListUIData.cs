using UnityEngine.Events;

namespace Project.Scripts.Common.UI;

public record class ListUIData<L, M>(M Model, UnityAction<L>? OnSelect = null) : IPresentable;
