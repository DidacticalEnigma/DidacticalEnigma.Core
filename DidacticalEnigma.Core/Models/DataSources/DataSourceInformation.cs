using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace DidacticalEnigma.Core.Models.DataSources
{
    [Serializable]
    public class DataSourceInformation
    {
        [Required] [NotNull] public string Identifier { get; }
        
        [Required] [NotNull] public string FriendlyName { get; }

        public DataSourceInformation([NotNull] string identifier, [NotNull] string friendlyName)
        {
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
            FriendlyName = friendlyName ?? throw new ArgumentNullException(nameof(friendlyName));
        }
    }
}