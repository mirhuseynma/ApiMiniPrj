namespace ApiMiniPrj.Domain.Models.Common
{
    public class AuditableEntity : BaseEntity
    {
        public string CreatedBy { get; set; } = string.Empty;
        public string DeletableBy { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set;} = DateTime.Now;
        public DateTime UpdatedDate { get; set;} 
        public DateTime DeletableDate { get; set; }
    }
}
