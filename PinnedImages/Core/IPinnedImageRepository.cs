using Data.Common.Contracts.SpecificationRepositories;
using Shared;

namespace Core
{
    public record PinnedImageByImageIdSpecification(ImageId ImageId) : ISpecification<Core.PinnedImage?>;

    public interface IPinnedImageRepository : 
        IAsyncRepository<Core.PinnedImage>, 
        IHandlesSpecificationAsync<PinnedImageByImageIdSpecification, Core.PinnedImage?>
    {

    }
}
