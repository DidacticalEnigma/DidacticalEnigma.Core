using System;
using Optional;

namespace JDict
{
    public class TatoebaUserSkill
    {
        public string Language { get; }

        public Option<int> SkillLevel { get; }

        public string Username { get; }

        public string Details { get; }

        public TatoebaUserSkill(string language, Option<int> skillLevel, string username, string details)
        {
            Language = language ?? throw new ArgumentNullException(nameof(language));
            SkillLevel = skillLevel;
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Details = details ?? throw new ArgumentNullException(nameof(details));
        }
    }
}