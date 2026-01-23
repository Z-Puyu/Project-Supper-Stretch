using CommonFrameworks.Events;

namespace GameplayKeywordsSystem.Runtime {
    public readonly record struct GameMessage(Keyword Key) : IMessage;
}
