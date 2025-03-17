using MongoDB.Driver;

namespace BobPastaBackend
{
    public class BobPastaService
    {
        private readonly IMongoCollection<BobPasto> _bobpastos;

        public BobPastaService(IMongoDatabase database)
        {
            _bobpastos = database.GetCollection<BobPasto>("bobpastos");
        }

        public async Task<List<BobPasto>> GetBobpastosAsync()
        {
            return await _bobpastos.Find(_ => true).ToListAsync();
        }

        public async Task<BobPasto> CreateBobPastoAsync(BobPasto bobPasto)
        {
            await _bobpastos.InsertOneAsync(bobPasto);
            return bobPasto;
        }

        public async Task<bool> DeleteAll()
        {
            try
            {
                await _bobpastos.DeleteManyAsync(_ => true);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
