using Project.Scripts.Common.UI;
using Project.Scripts.UI.Components;

namespace Project.Scripts.UI.Control;

public abstract class ProgressBarPresenter<M, P> : UIPresenter<M, ProgressBar, P> where P : IPresentable;
