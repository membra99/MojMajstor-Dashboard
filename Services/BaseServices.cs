using AutoMapper;
using Entities.Context;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BaseServices
    {
        protected readonly MainContext _context;
        protected readonly IMapper _mapper;
        protected readonly UsersServices _usersServices;


        public BaseServices(MainContext context)
        {
            _context = context;
        }

        public BaseServices(MainContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public BaseServices(MainContext context, IMapper mapper, UsersServices usersServices)
        {
            _context = context;
            _mapper = mapper;
            _usersServices = usersServices;
        }

        protected async Task SaveContextChangesAsync(IDbContextTransaction dbContextTransaction = null)
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (dbContextTransaction != null)
                {
                    dbContextTransaction.Rollback();
                }

                throw new Exception(e.Message);
            }
        }
    }
}
