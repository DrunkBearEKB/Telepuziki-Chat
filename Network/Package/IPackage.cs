namespace Network.Package
{
    public interface IPackage
    {
        PackageType Type { get; }
        string IdReceiver { get; }
        string IdAuthor { get; }
        byte[] RawData { get; }
    }
}