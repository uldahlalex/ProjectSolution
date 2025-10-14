using dataccess;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;

namespace api;

public class ApplicationSieveProcessor : SieveProcessor
{
    public ApplicationSieveProcessor(IOptions<SieveOptions> options) : base(options)
    {
    }

    public ApplicationSieveProcessor(IOptions<SieveOptions> options, ISieveCustomSortMethods customSortMethods) : base(options, customSortMethods)
    {
    }

    public ApplicationSieveProcessor(IOptions<SieveOptions> options, ISieveCustomFilterMethods customFilterMethods) : base(options, customFilterMethods)
    {
    }

    public ApplicationSieveProcessor(IOptions<SieveOptions> options, ISieveCustomSortMethods customSortMethods, ISieveCustomFilterMethods customFilterMethods) : base(options, customSortMethods, customFilterMethods)
    {
    }
    
    protected override SievePropertyMapper MapProperties(SievePropertyMapper mapper)
    {
        mapper.Property<Author>(p => p.Name)
            .CanFilter()
            .CanSort();

        return mapper;
    }
}