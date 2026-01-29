using LibraryServer.DbContext;

namespace LibraryServer.Service
{
    public class FavoriteBookUserService
    {
        private readonly LibraryContext _libraryContext;
        public FavoriteBookUserService(LibraryContext libraryContext)
        {
            _libraryContext = libraryContext;
        }
    }
}
