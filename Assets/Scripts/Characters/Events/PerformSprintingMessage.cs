using CommonFrameworks.Events;

namespace Characters.Events {
    public readonly record struct PerformSprintingMessage(bool IsSprinting) : IMessage;
}
