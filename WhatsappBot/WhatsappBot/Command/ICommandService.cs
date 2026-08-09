namespace WhatsappBot.Command;

public interface ICommandService
{
    void TriggerCommandFunction(CommandResult command);
}