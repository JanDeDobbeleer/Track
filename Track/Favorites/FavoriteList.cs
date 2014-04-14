using System.Collections.Generic;

namespace Track.Favorites
{
    public class FavoriteList
    {
        public IEnumerable<Favorite> Favorites { get; private set; }

        public void AddToFavorite(Favorite favorite)
        {
            //TODO: Implement this
        }

        #region Constructor
        private static FavoriteList s_Instance;
        private static object s_InstanceSync = new object();

        protected FavoriteList()
        {

        }

        public static FavoriteList GetInstance()
        {
            // This implementation of the singleton design pattern prevents 
            // unnecessary locks (using the double if-test)
            if (s_Instance == null)
            {
                lock (s_InstanceSync)
                {
                    if (s_Instance == null)
                    {
                        s_Instance = new FavoriteList();
                    }
                }
            }
            return s_Instance;
        }
        #endregion
    }
}
