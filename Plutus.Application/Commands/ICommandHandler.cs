namespace Plutus.Application.Commands;

public interface ICommandHandler<T> where T: ICommand
{
	Task Handle(T command);
}