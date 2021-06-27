using System;

namespace JDict
{
    public class TatoebaList
    {
        public long ListId { get; }

        public string Username { get; }

        public DateTime DateCreated { get; }

        public DateTime DateModified { get; }

        public string ListName { get; }

        public Editability EditableBy { get; }

        public TatoebaList(
            long listId,
            string username,
            DateTime dateCreated,
            DateTime dateModified,
            string listName,
            Editability editableBy)
        {
            ListId = listId;
            Username = username ?? throw new ArgumentNullException(nameof(username));
            DateCreated = dateCreated;
            DateModified = dateModified;
            ListName = listName ?? throw new ArgumentNullException(nameof(listName));
            EditableBy = editableBy;
        }
    }
}