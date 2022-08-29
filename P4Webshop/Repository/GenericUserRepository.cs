
using Microsoft.EntityFrameworkCore;
using P4Webshop.Interface.Generic;
using P4Webshop.Models;

namespace P4Webshop.Repositorys
{
    /// <summary>
    /// So how does generic repo work. GenericUserRepository<TEntity> is the same as writing GenericUserRepository<Customer>.
    /// It has ofc an Interface that also have TEntity to mimic this class. But after that at where TEntity : class
    ///  means that the class we put in at the TEntity place GenericUserRepository<Customer> it will initialize as that class.
    /// 
    /// So this becomes the crud for that user class, instead of we should write repo for each type of users.
    /// The IUserEntities is verrry important. You see it both here and on the user models. becuase when we code, TEntity is not a thing
    /// so to simulate that it has an ID property we set an interface on both repo and user models
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class GenericUserRepository<TEntity> : IGenericUserRepository<TEntity>
                    where TEntity : class, IUserEntities
    {
        private readonly MyDbContext _db;
        private readonly ILogger _logger;
        private DbSet<TEntity> _dbSet;
        public GenericUserRepository(MyDbContext repo, ILogger logger)
        {
            _db = repo;
            _logger = logger;
            _dbSet = _db.Set<TEntity>();

        }

        public async Task<bool> PostUserEntityAsync(TEntity entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{REPO} Post User method error", typeof(GenericUserRepository<TEntity>));
                return false;
            }
        }


        public async Task<bool> UpdateUserEntityAsync(TEntity entity)
        {
            try
            {
                _dbSet.Update(entity);
                return (await _db.SaveChangesAsync()) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{REPO} Update User method error", typeof(GenericUserRepository<TEntity>));
                return false;
            }
        }

        public async Task<bool> UpdateUserEntityListAsync(List<TEntity> entity)
        {
            try
            {
                _dbSet.UpdateRange(entity);
                return (await _db.SaveChangesAsync()) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{REPO} Update User List method error", typeof(GenericUserRepository<TEntity>));
                return false;
            }
        }



        public async Task<TEntity> GetUserEntityByIDAsync(Guid stringid)
        {
            try
            {
                return await _dbSet.FindAsync(stringid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{REPO} Get User method error", typeof(GenericUserRepository<TEntity>));
                return null;
            }
        }

        public async Task<bool> DeleteUserEntityAsync(Guid guid)
        {
            try
            {
                var exist = await _dbSet.Where(x => x.Id == guid).FirstOrDefaultAsync();

                if (exist != null)
                {
                    _dbSet.Remove(exist);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{REPO} Delete User method error", typeof(GenericUserRepository<TEntity>));
                return false;
            }
        }

        public async Task<List<TEntity>> GetAllUserEntitiesAsync()
        {
            try
            {
                return await _dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{REPO} GetAllUserEntitiesAsync method error", typeof(GenericUserRepository<TEntity>));
                return null;
            }
        }

        public async Task<TEntity> RefreshTokenUserEntity(string token)
        {
            try
            {
                return await _db.Set<TEntity>().FirstOrDefaultAsync(x => x.RefreshTokens.Any(y => y.Token == token));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{REPO} RefreshTokenUserEntity User method error", typeof(GenericUserRepository<TEntity>));
                return null;
            }
        }
    }
}
