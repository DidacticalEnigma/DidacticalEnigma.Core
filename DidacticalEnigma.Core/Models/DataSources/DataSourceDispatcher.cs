using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DidacticalEnigma.Core.Models.Formatting;
using Optional;
using Optional.Collections;

namespace DidacticalEnigma.Core.Models.DataSources
{
    public class DataSourceDispatcher
    {
        private readonly Dictionary<string, IDataSource> dataSources;

        private readonly Dictionary<string, DataSourceDescriptor> descriptors;

        public DataSourceDispatcher(IEnumerable<IDataSource> dataSources)
        {
            this.dataSources = new Dictionary<string, IDataSource>();
            this.descriptors = new Dictionary<string, DataSourceDescriptor>();
            foreach (var dataSource in dataSources)
            {
                if (dataSource.GetType().GetProperty("Descriptor", BindingFlags.Static | BindingFlags.Public)?.GetValue(null) is
                    DataSourceDescriptor descriptor)
                {
                    var dataSourceInstanceIdentifier = dataSource.InstanceIdentifier;
                    var dataSourceIdentifier = descriptor.Guid.ToString() + (dataSourceInstanceIdentifier != null ? "|" + dataSourceInstanceIdentifier : "");
                    this.dataSources.Add(dataSourceIdentifier, dataSource);
                    this.descriptors.Add(dataSourceIdentifier, descriptor);
                }
            }
        }

        public IEnumerable<DataSourceInformation> DataSourceIdentifiers =>
            descriptors
                .Select(kvp =>
                {
                    var kind = kvp.Key.Split('|').ElementAtOrDefault(1);
                    kind = kind != null ? $" ({kind})" : "";
                    return new DataSourceInformation
                    (
                        identifier: kvp.Key,
                        friendlyName: kvp.Value.Name + kind
                    );
                });

        public async Task<Option<RichFormatting>> GetAnswer(
            string dataSourceIdentifier,
            Request request,
            CancellationToken token = default)
        {
            var result = await dataSources
                .GetValueOrNone(dataSourceIdentifier)
                .Map(dataSource => dataSource.Answer(request, token))
                .ValueOr(() => Task.FromResult(Option.None<RichFormatting>()));
            return result;
        }
    }
}