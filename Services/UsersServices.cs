using AutoMapper;
using Entities.Context;
using Entities.Universal.MainData;
using Entities.Universal.MainDataNova;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Services.Authorization;
using Services.AWS;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Universal.DTO.IDTO;
using Universal.DTO.ODTO;
using Universal.Universal.MainDataNova;
using static Universal.DTO.CommonModels.CommonModels;

namespace Services
{
	public class UsersServices : BaseServices
	{
		private readonly IJwtUtils _jwtUtils;
		private readonly IAWSS3FileService _AWSS3FileService;
		private readonly IOptions<EmailSettings> _emailSettings;
        private readonly IOptions<URL> _urlDomain;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public static string AppBaseUrl = "";

		public UsersServices(MainContext context, IMapper mapper, IJwtUtils jwtUtils, IHttpContextAccessor httpContext, IAWSS3FileService AWSS3FileService, IOptions<EmailSettings> emailSettings, IOptions<URL> urlDomain, MojMajstorContext context2) : base(context,  mapper, context2)
		{
			_jwtUtils = jwtUtils;
			_AWSS3FileService = AWSS3FileService;
			_emailSettings = emailSettings;
			_httpContextAccessor = httpContext;
            _urlDomain = urlDomain;
            AppBaseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}";
		}

		#region Users

		public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
		{
			var user = await _context2.Users.SingleOrDefaultAsync(x => x.Email == model.Username && x.RoleId == 3);

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

        private IQueryable<UserMajstorODTO> GetMajstorUsers(int id)
        {
            return from x in _context2.Users
				   .Include(x => x.Role)
				   .Include(x => x.Opstine)
                   where (id == 0 || x.UsersId == id)
                   select _mapper.Map<UserMajstorODTO>(x);
        }

        public async Task<List<UserMajstorODTO>> GetAllMajstorUsers()
        {
            return await GetMajstorUsers(0).AsNoTracking().ToListAsync();
        }

		public async Task<UserMajstorIDTO> GetUserIDTO()
		{
			return new UserMajstorIDTO {
				OpstineDropDowns = await _context2.Opstines.Select(x => new OpstineDropDown { OpstineId = x.OpstineId, OpstineIme = x.OpstinaIme }).ToListAsync(),
				ProfessionDropDowns = await _context2.Professions.Select(x => new ProfessionDropDown { ProfessionId = x.ProfessionId, ProfessionName = x.ProfessionName }).ToListAsync()
			};
		}

        public async Task<ViewsByPeriodODTO> GetViewsByPeriod(DateTime startDate, DateTime endDate) 
		{
            var ads = _context2.Advertisements
         .Where(a => a.PostedDate >= startDate && a.PostedDate <= endDate)
         .ToList();

            var grouped = ads
                .GroupBy(a => new { a.PostedDate.Year, a.PostedDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    YearMonth = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Count = g.Count()
                }).ToList();

            var labels = grouped.Select(g => g.YearMonth.ToString("MMM yyyy")).ToList();
            var counts = grouped.Select(g => g.Count).ToList();

            return new ViewsByPeriodODTO
            {
                Labels = labels,
                Counts = counts
            };
        }


        public async Task<AdStatsODTO> GetAllAdvertisements()
		{
			var oglasi = await _context2.Advertisements.ToListAsync();
            AdStatsODTO adStatsODTO = new AdStatsODTO();
			adStatsODTO.TotalAds = oglasi.Count;
			adStatsODTO.Aktivni = oglasi.Count(x => x.IsActive == true);
			adStatsODTO.Istaknuti = oglasi.Count(x => x.IsActive == true && x.AdvertisementTypeId == 2);
			adStatsODTO.XL = oglasi.Count(x => x.AdvertisementTypeId == 3 && x.IsActive == true);

			//var profesije = oglasi.Select(x => x.ProfessionId).ToList();
			//foreach (var item in profesije)
			//{
			//	var profesijaIme = await _context2.Professions.Where(x => x.ProfessionId == item).Select(x => x.ProfessionName).SingleOrDefaultAsync();
			//	if (!adStatsODTO.Professions.Contains(profesijaIme))
			//	{
   //                 adStatsODTO.Professions.Add(profesijaIme);
   //             }
   //         }
			var clickpoprofesiji = oglasi.GroupBy(x => x.ProfessionId).Select(x => new {professionId = x.Key, TotalClick = x.Sum(x => x.ClickCount)}).ToList();
			foreach (var item in clickpoprofesiji)
			{
                adStatsODTO.Professions.Add(await _context2.Professions.Where(x => x.ProfessionId == item.professionId).Select(x => x.ProfessionName).SingleOrDefaultAsync());
				adStatsODTO.ViewsByProfession.Add(item.TotalClick);
            }
            return adStatsODTO;
        }

        private IQueryable<UsersIDTO> GetUsers(string password)
		{
			return from x in _context.Users
				   .Include(x => x.Media)
				   where (password == "" || x.Password == password)
				   select _mapper.Map<UsersIDTO>(x);
		}

        private IQueryable<UserMajstorIDTO> GetMajstorUsers(string password)
        {
            return from x in _context2.Users
                   where (password == "" || x.Password == password)
                   select _mapper.Map<UserMajstorIDTO>(x);
        }

        public async Task<List<UsersODTO>> GetAllUsers()
		{
			return await GetUsers(0).AsNoTracking().ToListAsync();
		}

		public async Task<string> RedirectLink(string key)
		{
			var user = await _context.Users.Where(x => x.Password == key).SingleOrDefaultAsync();
			if (user != null)
			{
				user.IsActive = true;
				_context.Entry(user).State = EntityState.Modified;
				await SaveContextChangesAsync();

				return "https://www.google.com";
			}
			return null;

		}

        public async Task<UserMajstorODTO> GetMajstorUserById(int id)
        {
            return await GetMajstorUsers(id).AsNoTracking().SingleOrDefaultAsync();
        }

        public async Task<UsersODTO> GetUserById(int id)
		{
			return await GetUsers(id).AsNoTracking().SingleOrDefaultAsync();
		}

		public async Task<MetricByUserODTO> GetMetricByUser(int userId)
		{
			var user = await _context2.Users.Where(x => x.UsersId == userId).SingleOrDefaultAsync();
            var oglasi = await _context2.Advertisements.CountAsync(x => x.UsersId == userId && x.IsActive == true);
            var dogovori = await _context2.MakeDeals.CountAsync(x => x.FirstUserAccept == true && x.SecondUserAccept == true && (x.FirstUserId == userId || x.SecondUserId == userId));
            var brojNepostignutihDogovora = await _context2.MakeDeals.CountAsync(x => (x.FirstUserAccept == true && x.SecondUserAccept == false || x.FirstUserAccept == false && x.SecondUserAccept == true) && (x.FirstUserId == userId || x.SecondUserId == userId));

            DateTime today = DateTime.Today;

            #region Aktivnost
            int months = (today.Year - user.RegistrationDate.Year) * 12 + today.Month - user.RegistrationDate.Month;
            if (today.Day >= user.RegistrationDate.Day)
            {
                months++;
            }
            #endregion
           
            MetricByUserODTO metrics = new MetricByUserODTO();
			metrics.Aktivnost = (oglasi + dogovori) / months;
			metrics.Pouzdanost = (double)(dogovori - brojNepostignutihDogovora) / dogovori;

            return metrics;
        }


        public async Task<UserMajstorIDTO> GetUserByIdForEdit(int id)
		{
			var user = _mapper.Map<UserMajstorIDTO>(await _context2.Users.Include(x => x.Opstine).Where(x => x.UsersId == id).AsNoTracking().SingleOrDefaultAsync());

			var oglasi = await _context2.Advertisements.Where(x => x.UsersId == id).ToListAsync();

			user.UkupanBrOglasa = oglasi.Count;
			user.TrenutnoAktivniOglasi = oglasi.Count(x => x.IsActive == true);
            user.BrojPostignutihDogovora = await _context2.MakeDeals.CountAsync(x => x.FirstUserAccept == true && x.SecondUserAccept == true && (x.FirstUserId == id || x.SecondUserId == id));
            user.BrojNepostignutihDogovora = await _context2.MakeDeals.CountAsync(x => (x.FirstUserAccept == true && x.SecondUserAccept == false || x.FirstUserAccept == false && x.SecondUserAccept == true) && (x.FirstUserId == id || x.SecondUserId == id));
			user.BrojKorisnikaPrekoReferala = await _context2.Invitations.CountAsync(x => x.OriginUserId == id);

			var brkupljenihpaketa = await _context2.Orders.Where(x => x.UsersId == id).Select(x => x.TokenId).ToListAsync();
			user.BrojKupljenihPateka = new BrojKupljenihPateka();
			foreach (var item in brkupljenihpaketa)
			{
				switch (item)
				{
					case 1:
						user.BrojKupljenihPateka.S += 1;
						break;

                    case 2:
                        user.BrojKupljenihPateka.M += 1;
                        break;

                    case 3:
                        user.BrojKupljenihPateka.L += 1;
                        break;

                    case 4:
                        user.BrojKupljenihPateka.XL += 1;
                        break;
                }
			}

            return user;
		}

		public async Task<UserMajstorIDTO> GetUserByPassword(string password)
		{
			return await GetMajstorUsers(password).AsNoTracking().SingleOrDefaultAsync();
		}

        public async Task<UserMajstorODTO> AddUserMojMajstor(UserMajstorIDTO userIDTO)
		{
            var CheckUser = await _context2.Users.Where(x => x.Email == userIDTO.Email).FirstOrDefaultAsync();
            if (CheckUser != null)
            {
                return null;
            }
			userIDTO.Professions = string.Join(",", userIDTO.SelectedProfessionIds);
            var user = _mapper.Map<User>(userIDTO);
			user.RegistrationDate = DateTime.Now;
            user.IsActive = true;

            user.UsersId = 0;
			user.IsActive = true;
			user.ReferalCode = GenerateSecureRandomCode(10);
            user.Password = BCrypt.Net.BCrypt.HashPassword("tempPasswordFix4You123456789012301231");
            _context2.Users.Add(user);

            await SaveContextChangesMajstorAsync();

            return await GetMajstorUserById(user.UsersId);
        }

        static string GenerateSecureRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Range(0, length)
                .Select(_ => chars[RandomNumberGenerator.GetInt32(chars.Length)]).ToArray());
        }

        public async Task<UsersODTO> AddUser(UsersIDTO userIDTO)
		{
			var CheckUser = await _context.Users.Where(x => x.Email == userIDTO.Email).FirstOrDefaultAsync();
			if (CheckUser != null)
			{
				return null;
			}
			var user = _mapper.Map<Users>(userIDTO);
			user.IsActive = true;

			//initial user set, password is temp and user is instructed to change their password by mail
			user.UsersId = 0;
			user.Password = BCrypt.Net.BCrypt.HashPassword("tempPasswordDOTUniversalABC123456789012301231");

			MailService ms = new MailService(_emailSettings);
			string userKey = user.Password; // CHANGE IF NEEDED TO SOMETHING ELSE
			ms.SendEmail(new EmailIDTO
			{
				To = user.Email,
				Subject = user.FirstName + ", please set your password in order to access your new account!",
				Body = "Press the activation link to set your password and gain access to your account. <br> <a href='" + AppBaseUrl + "/Dashboard/SetPassword?key=" + userKey + "'>Set your password</a>"
			});
			try
			{
				_context.Users.Add(user);

				await SaveContextChangesAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}

			return await GetUserById(user.UsersId);
		}

		public async Task<string> RegisterUser(UsersRegisterIDTO userIDTO)
		{
			string pattern = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&.])[A-Za-z\\d@$!%*?&.]{8,}$";
			var CheckUser = await _context.Users.Where(x => x.Email == userIDTO.Email).FirstOrDefaultAsync();
			if (CheckUser != null)
			{
				return "EmailExists";
			}
			if (!Regex.IsMatch(userIDTO.Password, pattern))
			{
				return "RegexException";
			}
			var user = _mapper.Map<Users>(userIDTO);

			//initial user set, password is temp and user is instructed to change their password by mail
			user.UsersId = 0;
			user.Password = BCrypt.Net.BCrypt.HashPassword("tempPasswordDOTUniversalABC123456789012301231");

			MailService ms = new MailService(_emailSettings);
			string userKey = user.Password; // CHANGE IF NEEDED TO SOMETHING ELSE
			ms.SendEmail(new EmailIDTO
			{
				To = user.Email,
				Subject = user.FirstName + ", please activate your account!",
				Body = "Thank you for registering on our site.<br> To verify your registration, click on this link: <br> <a href='"+_urlDomain.Value.MainUrl+"api/Users/Redirect?key=" + userKey + "'>Activate Your Mail</a>"
			});
			_context.Users.Add(user);

			await SaveContextChangesAsync();

			return "Done";
		}

		public async Task<MediaODTO> UploadUserPicture(AWSFileUpload awsFile, int? mediatypeId)
		{
			string successUpload = "";

			if (awsFile.Attachments.Count > 0)
				successUpload = await _AWSS3FileService.UploadFile(awsFile);

			if (successUpload != null)
			{
				var key = await _AWSS3FileService.FilesListSearch(successUpload);
				var media = new Media();
				media.Extension = successUpload.Split('.')[1];
				media.Src = key.First();
				media.MediaTypeId = (mediatypeId == null) ? _context.MediaTypes.Where(x => x.MediaTypeName == "Avatar").Select(x => x.MediaTypeId).FirstOrDefault() : (int)mediatypeId;
				var index = media.Src.LastIndexOf('/');
				media.MetaTitle = media.Src.Substring(index + 1);
				media.MimeType = awsFile.Attachments[0].ContentType;
				try
				{
					_context.Medias.Add(media);
					await _context.SaveChangesAsync();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}

				return _mapper.Map<MediaODTO>(media);
			}
			else
			{
				return null;
			}
		}

		public async Task<UserMajstorODTO> EditUser(UserMajstorIDTO userIDTO)
		{
			var user = _mapper.Map<User>(userIDTO);
			_context2.Entry(user).State = EntityState.Modified;
            _context2.Entry(user).Property(x => x.Password).IsModified = false;
            _context2.Entry(user).Property(x => x.OpstineId).IsModified = false;
            _context2.Entry(user).Property(x => x.RegistrationDate).IsModified = false;
            user.IsActive = true;
			await SaveContextChangesMajstorAsync();

			return await GetMajstorUserById(user.UsersId);
		}

        public async Task<string> SendPasswordResetMail(string userMail)
        {
            var oldPaswd = await _context.Users.Where(x => x.Email == userMail).FirstOrDefaultAsync();
            if (oldPaswd == null)
                return "Korisnik nije pronađen";

            MailService ms = new MailService(_emailSettings);
            ms.SendEmail(new EmailIDTO
            {
                To = userMail,
                Subject = "Reset password",
                Body = "Hello, we have received a request to change your password. If you did not initiate this request, feel free to ignore this email" + "<br/>" +
                "If you have requested a password change, click on the following link and enter your new password " + "<a href='https://localhost:7213/Dashboard/SetPassword?key=" + oldPaswd.Password + "'> Change Password</a>"
            });

            return "Password reset mail has been sent to your email address";
        }

        public async Task<UsersODTO> DeleteUser(int id)
		{
			var user = await _context.Users.FindAsync(id);
			if (user == null) return null;

			var userODTO = await GetUserById(id);

			var orders = await _context.Orders.Where(x => x.UsersId == id).ToListAsync();
			foreach (var order in orders)
			{
				_context.Orders.Remove(order);
			}

			_context.Users.Remove(user);
			await SaveContextChangesAsync();

			return userODTO;
		}

		#endregion Users
	}
}