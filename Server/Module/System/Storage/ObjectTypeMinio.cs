using Minio.DataModel;
namespace System.Storage;

public class ObjectTypeMinio
{
    public ObjectStat FileStat { get; set; }= null!;
    public byte[] Data { get; set; } = null!;
}