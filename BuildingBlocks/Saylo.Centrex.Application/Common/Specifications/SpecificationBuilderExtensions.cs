using Saylo.Centrex.Domain.Entities;
using Saylo.Centrex.Domain.Specifications;

namespace Saylo.Centrex.Application.Common.Specifications;

public static class SpecificationBuilderExtensions
{
    public static SpecificationBuilder<T, TResult> CreateSpecification<T, TResult>(this IProjectionProvider projectionProvider)
        where T : class, IAggregateRoot
        => new(projectionProvider);

    public static SpecificationBuilder<T, TResult> If<T, TResult>(
        this SpecificationBuilder<T, TResult> builder,
        bool condition,
        Func<SpecificationBuilder<T, TResult>, SpecificationBuilder<T, TResult>> action)
        where T : class, IAggregateRoot
        => condition ? action(builder) : builder;
}