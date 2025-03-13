using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Internal;
using Saylo.Centrex.Domain.Specifications;

namespace Saylo.Centrex.Application.Common.Automapper;

public class AutoMapperProjectionProvider : IProjectionProvider
{
    private readonly IMapper _mapper;

    public AutoMapperProjectionProvider(IMapper mapper)
    {
        _mapper = mapper;
    }

    public Expression<Func<TSource, TDestination>> GetProjection<TSource, TDestination>()
    {
        return source =>  _mapper.Map<TDestination>(source);
    }

    public TDestination Map<TSource, TDestination>(TSource source)
    {
        return _mapper.Map<TDestination>(source);
    }
}