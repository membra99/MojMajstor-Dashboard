using AutoMapper;
using Entities.Context;
using Entities.Universal.MainData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universal.DTO.IDTO;
using Universal.DTO.ODTO;

namespace Services
{
    public class UsersServices : BaseServices
    {
        public UsersServices(MainContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #region Users

        private IQueryable<UsersODTO> GetUsers(int id)
        {
            return from x in _context.Users
                   .Include(x => x.Media)
                   where (id == 0 || x.UsersId == id)
                   select _mapper.Map<UsersODTO>(x);
        }

        public async Task<UsersODTO> GetUserById(int id)
        {
            return await GetUsers(id).AsNoTracking().SingleOrDefaultAsync();
        }

        public async Task<UsersODTO> AddUser(UsersIDTO userIDTO)
        {
            var user = _mapper.Map<Users>(userIDTO);
            user.UsersId = 0;
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _context.Users.Add(user);

            await SaveContextChangesAsync();

            return await GetUserById(user.UsersId);
        }

        public async Task<UsersODTO> EditUser(UsersIDTO userIDTO)
        {
            var user = _mapper.Map<Users>(userIDTO);
            _context.Entry(user).State = EntityState.Modified;
            await SaveContextChangesAsync();

            return await GetUserById(user.UsersId);
        }

        public async Task<UsersODTO> DeleteUser(int id)
        {
            var user = await _context.Products.FindAsync(id);
            if (user == null) return null;

            var userODTO = await GetUserById(id);

            _context.Products.Remove(user);
            await SaveContextChangesAsync();

            return userODTO;
        }

        #endregion
    }
}
