
namespace System.Storage;

public class StorageRes
{
    public string TenantResourceId { get; set; } = null!;
    public string TenantId { get; set; } = null!;
    public string BucketName { get; set; }
    public decimal? BucketQuota { get; set; }
    public decimal? BucketUsage { get; set; }
    public decimal? BucketObject { get; set; }
    public DateTime? BucketCreationDate { get; set; }

}