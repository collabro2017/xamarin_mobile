using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace X
{ 
    public class slqLiteHelper
    {
        readonly SQLiteAsyncConnection database;

        public slqLiteHelper(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Emojis>().Wait();
        }

        public Task<List<Emojis>> GetItemsAsync()
        {
            return database.Table<Emojis>().ToListAsync();
        }

        //public Task<List<SavedLocationsItems>> GetItemsNotDoneAsync()
        //{
        //  return database.QueryAsync<SavedLocationsItems>("SELECT * FROM [SavedLocationsItems] WHERE [Done] = 0");
        //}

        public Task<Emojis> GetItemAsync(int id)
        {
            return database.Table<Emojis>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(Emojis item)
        {
            if (item.ID != 0)
            {
                return database.UpdateAsync(item);
            }
            else
            {
                return database.InsertAsync(item);
            }
        }


        public Task<int> DeleteItemAsync(Emojis item)
        {
            return database.DeleteAsync(item);
        }
    }
}
