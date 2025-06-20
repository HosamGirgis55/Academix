using MediatR;

namespace Academix.Application.Common.Interfaces
{
    // Marker interface for commands
    public interface ICommand : IRequest
    {
    }

    // Generic command with response
    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    }

    // Command handler interfaces
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
        where TCommand : ICommand
    {
    }

    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
    }
} 