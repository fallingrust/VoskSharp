namespace VoskSharp.LibraryLoader
{
    public interface ILibraryLoader
    {
        LoadResult OpenLibrary(string? fileName);
    }
}
