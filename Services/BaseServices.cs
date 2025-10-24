using AutoMapper;
using Entities.Context;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Universal.MainDataNova;

namespace Services
{
	public class BaseServices
	{
		protected readonly MainContext _context;
		protected readonly MojMajstorContext _context2;
		protected readonly IMapper _mapper;
		protected readonly UsersServices _usersServices;


		public BaseServices(MainContext context, MojMajstorContext context2)
		{
			_context = context;
			_context2 = context2;
		}

		public BaseServices(MainContext context, IMapper mapper, MojMajstorContext context2)
		{
			_context = context;
			_mapper = mapper;
			_context2 = context2;
        }

		public BaseServices(MainContext context, IMapper mapper, UsersServices usersServices, MojMajstorContext context2)
		{
			_context = context;
			_mapper = mapper;
			_usersServices = usersServices;
			_context2 = context2;

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

        protected async Task SaveContextChangesMajstorAsync(IDbContextTransaction dbContextTransaction = null)
        {
            try
            {
                await _context2.SaveChangesAsync();
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
