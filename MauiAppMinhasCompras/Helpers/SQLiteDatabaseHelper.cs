using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MauiAppMinhasCompras.Models;
using SQLite;

namespace MauiAppMinhasCompras.Helpers
{
    public class SQLiteDatabaseHelper
    {
        private readonly SQLiteAsyncConnection _conn;

        public SQLiteDatabaseHelper(string path)
        {
            _conn = new SQLiteAsyncConnection(path);
            _conn.CreateTableAsync<Produto>().Wait();
        }

        public Task<List<Produto>> GetAll()
        {
            return _conn.Table<Produto>().ToListAsync();
        }

        public Task<List<Produto>> Search(string filtro)
        {
            // Busca feita usando LIKE ignorando case, se quiser
            return _conn.Table<Produto>()
                        .Where(p => p.Descricao.ToLower().Contains(filtro.ToLower()))
                        .ToListAsync();
        }

        public Task<int> Insert(Produto p)
        {
            return _conn.InsertAsync(p);
        }

        public Task<int> Update(Produto p)
        {
            return _conn.UpdateAsync(p);
        }

        public async Task<int> Delete(int id)
        {
            var produto = await _conn.Table<Produto>()
                                     .Where(x => x.Id == id)
                                     .FirstOrDefaultAsync();
            if (produto != null)
                return await _conn.DeleteAsync(produto);
            return 0;
        }
    }
}
