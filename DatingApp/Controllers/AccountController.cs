﻿using AutoMapper;
using DatingApp.Data;
using DatingApp.DTO;
using DatingApp.Interface;
using DatingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseAPIController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService ,IMapper mapper)
        {
            
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")] //POST : api/account/register

        public async Task<ActionResult<UserDto>> Register([FromBody]RegisterDto registerDto)
        {
            if ((registerDto.Username == null) || (registerDto.Password == null))
            {
                return BadRequest("Feild should not be empty");
            }
            if (await UserExists(registerDto.Username))
            {
                return BadRequest("Username is taken");
            }
           
            
            using var hmac = new HMACSHA512();
            var user = new AppUser()
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),

            };




        }

        [HttpPost("login")] //POST : api/account/login
        public async Task<ActionResult<UserDto>> Login (LoginDto loginDto)
        {
            var user = await _context.Users
                .Include(p =>p.Photos)
                
                .SingleOrDefaultAsync(x =>x.UserName== loginDto.Username);

            if(user == null) 
            {
                return Unauthorized("Invalid Username"); 
            }
            if(user.UserName==null || user.PasswordHash == null)
            {
                return Unauthorized(" Feild should not be empty");
            }
            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0;i< computedHash.Length;i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("Invalid Password");
                }
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,

            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x =>x.UserName==username.ToLower());

        }



    }
}
