using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace DidacticalEnigma.Core.Models.HighLevel
{
    public class Error
    {
        [Required] [NotNull] public string Code { get; }
        
        [Required] [NotNull] public string Message { get; }
        
        [Required] [NotNull] public IReadOnlyCollection<string> Context { get; }

        public Error(
            [NotNull] string code,
            [NotNull] string message,
            [NotNull] IReadOnlyCollection<string> context = null)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Context = context ?? Array.Empty<string>();
        }
    }
}