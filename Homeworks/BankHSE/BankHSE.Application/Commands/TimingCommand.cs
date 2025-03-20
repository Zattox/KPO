using BankHSE.Domain.Abstractions;

namespace BankHSE.Application.Commands;

public class TimingCommand : ICommand
{
    private readonly ICommand _innerCommand;
    public TimeSpan Duration { get; private set; }

    public TimingCommand(ICommand innerCommand) => _innerCommand = innerCommand;

    public void Execute()
    {
        var startTime = DateTime.Now;
        _innerCommand.Execute();
        Duration = DateTime.Now - startTime;
        Console.WriteLine($"Command executed in {Duration.TotalMilliseconds} ms");
    }
}