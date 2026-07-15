namespace WhatsappBot.Command;

public interface ICommandService
{
    Task<HttpResponseMessage> TriggerCommandFunction(CommandResult command);
}