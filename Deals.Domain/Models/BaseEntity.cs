using static Deals.Domain.Constants.DomainConstants;

namespace Domain.Models
{
    /// <summary>
    /// Base class for all Domain entities.
    /// </summary>
    public abstract class BaseEntity
    {
        public Guid UUID { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        /// <summary>
        /// To be removed
        /// </summary>
        public bool IsDeleted { get; set; }
        public int StatusId { get; set; }

        public void Initialize()
        {
            SetCreated();
            SetModified();
        }

        public void Initialize(string userEmail)
        {
            SetCreated(userEmail);
            SetModified(userEmail);
        }

        public void SetCreated(string userEmail = "")
        {
            StatusId = (int)ItemStatus.Active;
            CreatedOn = DateTime.UtcNow;
            CreatedBy = userEmail;
        }

        public void SetModified(string userEmail = "")
        {
            ModifiedBy = userEmail;
            ModifiedOn = DateTime.UtcNow;
        }

        public void Delete()
        {
            IsDeleted = true;
        }

    }
}
