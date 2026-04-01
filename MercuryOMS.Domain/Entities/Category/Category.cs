using MercuryOMS.Domain.Commons;

namespace MercuryOMS.Domain.Entities
{
    public class Category : AggregateRoot, IAuditableUser
    {
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }

        private Category() { }

        public Category(string name, string description)
        {
            Id = Guid.NewGuid();
            SetName(name);
            SetDescription(description);
        }

        public void Update(string name, string description)
        {
            SetName(name);
            SetDescription(description);
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be empty.");

            if (name.Length > 200)
                throw new ArgumentException("Category name is too long.");

            Name = name;
        }

        public void SetDescription(string description)
        {
            Description = description?.Trim() ?? string.Empty;
        }
    }
}
