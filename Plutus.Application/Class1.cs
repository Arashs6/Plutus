namespace Plutus.Application;

public class PaymentService:ICommandHandler<PaymentOrderCreatedCommand>
{
	public Task Handle(PaymentOrderCreatedCommand command)
	{
		throw new NotImplementedException();
	}
}

public class PaymentOrderCreatedCommand : ICommand
{
	public Guid OrderId { get; set; }
	public decimal Amount { get; set; }
}

public interface ICommandHandler<T> where T: ICommand
{
	Task Handle(T command);
}


public interface ICommand{}