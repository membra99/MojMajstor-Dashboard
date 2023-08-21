using AutoMapper;
using Entities.Context;
using Entities.Universal.MainData;
using Microsoft.EntityFrameworkCore;
using Services.Authorization;
using Services.AWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universal.DTO.IDTO;
using Universal.DTO.ODTO;
using Universal.DTO.ViewDTO;
using static Universal.DTO.CommonModels.CommonModels;

namespace Services
{
    public class UsersServices : BaseServices
    {
        private readonly IJwtUtils _jwtUtils;
        private readonly IAWSS3FileService _AWSS3FileService;

        public UsersServices(MainContext context, IMapper mapper, IJwtUtils jwtUtils, IAWSS3FileService AWSS3FileService) : base(context, mapper)
        {
            _jwtUtils = jwtUtils;
            _AWSS3FileService = AWSS3FileService;
        }

        #region Users

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == model.Username);

            if (user != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Password)) user = null; //failed password authentication
            }

            // return null if user not found
            if (user == null) return null;

            // authentication successful so generate jwt token
            var token = _jwtUtils.GenerateJwtToken(user);

            AuthenticateResponse authenticateResponse = new AuthenticateResponse(user, token);
            return authenticateResponse;
        }

        private IQueryable<UsersODTO> GetUsers(int id)
        {
            return from x in _context.Users
                   .Include(x => x.Media)
                   where (id == 0 || x.UsersId == id)
                   select _mapper.Map<UsersODTO>(x);
        }

		public async Task<List<UsersODTO>> GetAllUsers()
		{
			return await GetUsers(0).AsNoTracking().ToListAsync();
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

        public async Task<MediaODTO> UploadUserPicture(AWSFileUpload awsFile)
        {
            bool successUpload = false;

            if (awsFile.Attachments.Count > 0)
                successUpload = await _AWSS3FileService.UploadFile(awsFile);

            if (successUpload)
            {
                var key = await _AWSS3FileService.FilesListSearch("DOT/" + awsFile.Attachments.First().FileName);
                var media = new Media();
                media.Extension = awsFile.Attachments.First().FileName.Split('.')[1];
                media.Src = key.First();
                media.MediaTypeId = _context.MediaTypes.FirstOrDefault(x => x.MediaTypeName == "Avatar").MediaTypeId;
                _context.Medias.Add(media);
                await _context.SaveChangesAsync();
                return _mapper.Map<MediaODTO>(media);
            }
            else
            {
                return null;
            }
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

        #endregion Users
    }
}