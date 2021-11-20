using System;
using System.Threading;
using System.Threading.Tasks;
using DidacticalEnigma.Core.Models.Formatting;
using Optional;

namespace DidacticalEnigma.Core.Models.DataSources
{
    public interface IDataSource : IDisposable
    {
        // It is expected that a class implementing this interface will provide a static property
        // of type DataSourceDescriptor

        Task<Option<RichFormatting>> Answer(Request request, CancellationToken token);

        Task<UpdateResult> UpdateLocalDataSource(
            CancellationToken cancellation = default(CancellationToken));
        
        // an identifer that is unique to the instance of the data source with given Descriptor.Guid
        // together with the Descriptor.Guid, it uniquely identifies an instance of data source
        // can be null
        string InstanceIdentifier { get; }
    }
}
