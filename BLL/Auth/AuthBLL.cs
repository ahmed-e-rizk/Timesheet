using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BLL.BaseResponse;
using DTO.Auth;
using DTO.Eunms;
using Helper;
using log4net;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repositroy;
using Timesheet.BLL.Validation.Auth;
using Timesheet.Core.Entites;
using Timesheet.DTO.Auth;
using Timesheet.Helper;
using Timesheet.Repositroy.Infrastructure;

namespace BLL.Auth
{
    public class AuthBLL :IAuthBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly AuthSetting _authSetting;

        private readonly IPasswordHasher _passwordHasher;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<RefreshToken> _refreshTokenRepository;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(AuthBLL));
        public AuthBLL(IUnitOfWork UnitOfWork, IMapper mapper, IOptions<AuthSetting> authSetting, IPasswordHasher PasswordHasher, IRepository<User> UserRepository, IRepository<RefreshToken> refreshTokenRepository)
        {
            _unitOfWork = UnitOfWork;
            _mapper = mapper;
            _authSetting = authSetting.Value;
            _passwordHasher = PasswordHasher;
            _userRepository = UserRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }
        public async Task<IResponse<bool>> RegisterAsync(RegisterDto registerDto)
        {
            var response = new Response<bool>();
            try
            {
                _logger.Info("Register Strat");
                var validation = await new RegisterDtoValidator().ValidateAsync(registerDto);
                if (!validation.IsValid)
                {
                    return response.CreateResponse(validation.Errors);
                }

                if (await _userRepository.AnyAsync(e => e.Email.ToLower().Equals(registerDto.Email.ToLower())))
                    return response.CreateResponse(MessageCodes.AlreadyExists);
                registerDto.Password = _passwordHasher.HashPassword(registerDto.Password);
                await _userRepository.AddAsync(_mapper.Map<User>(registerDto));

                await _unitOfWork.CommitAsync();
                return response.CreateResponse(true);
            }
            catch (Exception e)
            {
                _logger.Debug($"Register {e}");
                

                return response.CreateResponse(e);

            }


        }



        public async Task<IResponse<TokenResultDto>> LoginAsync(LoginDto loginDto)
        {
            var response = new Response<TokenResultDto>();
            try
            {                _logger.Info("Login Strat");

                var validation = await new LoginDtoValidator().ValidateAsync(loginDto);

                if (!validation.IsValid)
                {
                    response.CreateResponse(validation.Errors);
                    return response;
                }

                var user = await _userRepository.GetAsync(e => e.Email.ToLower().Equals(loginDto.Email.ToLower()));


                if (user is null)
                {
                    response.CreateResponse(MessageCodes.InvalidLoginCredentials);
                    return response;
                }


                if (!_passwordHasher.VerifyHashedPassword(loginDto.Password, user.Password))
                {
                    response.CreateResponse(MessageCodes.InvalidLoginCredentials);
                    return response;
                }

                var generatedJwtToken = await GenerateJwtTokenAsync(user);

                var generatedRefreshToken = await GenerateRefreshTokenAsync(generatedJwtToken.Jti, user.Id);

                var tokenResultDto = new TokenResultDto
                {
                    Token = generatedJwtToken.Token,
                    RefreshToken = generatedRefreshToken
                };

                return response.CreateResponse(tokenResultDto);
            }
            catch (Exception ex)
            {
                _logger.Debug($"Login {ex}");

                return response.CreateResponse(ex);
            }

        }


        public async Task<IResponse<TokenResultDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            var response = new Response<TokenResultDto>();

            try
            {
                _logger.Info("RefreshToken Strat");

                var validation = await new RefreshTokenDtoValidator().ValidateAsync(refreshTokenDto);

                if (!validation.IsValid)
                {

                    return response.CreateResponse(validation.Errors);
                }

                var TokenResult = await _refreshTokenRepository.GetAsync(rt => rt.Token == refreshTokenDto.RefreshToken);





                // generate new tokens.
                var user = await _userRepository.GetAsync(e => e.Id == TokenResult.UserId);

                if (user is null)
                {

                    return response.CreateResponse(MessageCodes.NotFound);
                }

                var newJwtToken = await GenerateJwtTokenAsync(user);

                var newRefreshToken = await UpdateRefreshTokenAsync(TokenResult, newJwtToken.Jti);

                var tokenResultDto = new TokenResultDto
                {
                    Token = newJwtToken.Token,
                    RefreshToken = newRefreshToken
                };

                return response.CreateResponse(tokenResultDto);
            }
            catch (Exception ex)
            {
                _logger.Debug($"Refresh token {ex}");

                return response.CreateResponse(ex);
            }

        }

        public async Task<IResponse<bool>> LogoutAsync(LogoutDto logoutDto)
        {
            var response = new Response<bool>();

            try
            {
                _logger.Info("LogOut strat");

                var validation = await new LogoutDtoValidator().ValidateAsync(logoutDto);

                if (!validation.IsValid)
                {
                    response.CreateResponse(validation.Errors);
                    return response;
                }

                var existRefreshToken = await _refreshTokenRepository.GetAsync(rt => rt.Token == logoutDto.RefreshToken);

                if (existRefreshToken is null)
                {
                    response.CreateResponse(MessageCodes.NotFound);
                    return response;
                }

                _refreshTokenRepository.Delete(existRefreshToken);
                await _unitOfWork.CommitAsync();

                response.CreateResponse(true);
            }
            catch (Exception ex)
            {
                _logger.Debug($"LogOut {ex}");

                response.CreateResponse(ex);
            }

            return response;
        }


        private async Task<JwtTokenDto> GenerateJwtTokenAsync(User user)
        {
            return await Task.Run(() =>
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();

                var key = Encoding.ASCII.GetBytes(_authSetting.Jwt.Secret);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Issuer = _authSetting.Jwt.Issuer,
                    Subject = new ClaimsIdentity(new List<Claim>
                    {
                        new Claim(TokenClaimTypeEnum.Id.ToString(), user.Id.ToString()),
                        new Claim(TokenClaimTypeEnum.Email.ToString(), user.Email),
                        new Claim(TokenClaimTypeEnum.Name.ToString(), user.Name),

                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    }),
                    Expires = DateTime.UtcNow.Add(_authSetting.Jwt.TokenExpiryTime),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };


                var token = jwtTokenHandler.CreateToken(tokenDescriptor);

                var jwtToken = jwtTokenHandler.WriteToken(token);

                return new JwtTokenDto
                {
                    Jti = token.Id,
                    Token = jwtToken,
                };
            });
        }
        private async Task<string> GenerateRefreshTokenAsync(string jti, int Id)
        {
            var refreshToken = new RefreshToken
            {
                Jti = jti,
                UserId = Id,
                ExpireDate = DateTime.UtcNow.AddMonths(_authSetting.Jwt.RefreshToken.RefreshTokenExpiryInMonths),
                CreateDate = DateTime.UtcNow,
                Token = $"{GenerateRandom(_authSetting.Jwt.RefreshToken.TokenLength)}{Guid.NewGuid()}"
            };

            await _refreshTokenRepository.AddAsync(refreshToken);
            await _unitOfWork.CommitAsync();

            return refreshToken.Token;
        }
        private string GenerateRandom(int length)
        {
            var random = new Random();

            return new string(Enumerable.Repeat("0123456789ABCDEFGHIJ0123456789KLMNOPQRST0123456789UVWXYZ012345", length)
                                        .Select(s => s[random.Next(s.Length)])
                                        .ToArray());
        }

        private async Task<string> UpdateRefreshTokenAsync(RefreshToken refreshToken, string jti)
        {
            refreshToken.Jti = jti;
            refreshToken.Token = $"{GenerateRandom(_authSetting.Jwt.RefreshToken.TokenLength)}{Guid.NewGuid()}";

            _refreshTokenRepository.Update(refreshToken);
            await _unitOfWork.CommitAsync();

            return refreshToken.Token;
        }
    }
}