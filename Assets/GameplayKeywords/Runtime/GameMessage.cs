using CommonFrameworks.Events;

namespace GameplayKeywords {
    public readonly record struct GameMessage(Keyword Key) : IMessage;
}
