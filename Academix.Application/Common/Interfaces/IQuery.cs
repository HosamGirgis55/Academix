using MediatR;

namespace Academix.Application.Common.Interfaces
{
    // Query interface - all queries must return a response
    public interface IQuery<out TResponse> : IRequest<TResponse>
    {
    }

    // Query handler interface
    public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
    }
} 