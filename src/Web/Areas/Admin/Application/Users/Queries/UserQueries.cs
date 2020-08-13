﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web.Areas.Admin.Application.Users.Queries.DTO;
using Web.Domain.Entities.Identity;

namespace Web.Areas.Admin.Application.Users.Queries
{
    public class UserQueries : IUserQueries
    {
        private readonly UserManager<User> _userManager;

        private static readonly IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<User, UserListItemDTO>();
            x.CreateMap<User, UserDetailsDTO>();
        }));

        
        public UserQueries(UserManager<User> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<IEnumerable<UserListItemDTO>> GetUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return _mapper.Map<List<User>, List<UserListItemDTO>>(users);
        }

        public async Task<UserDetailsDTO> GetUserAsync(string userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(f => f.Id == userId);
            if (user == null)
            {
                return null;
            }
            var userRoles = await _userManager.GetRolesAsync(user);

            var userDTO = _mapper.Map<User, UserDetailsDTO>(user);
            userDTO.Roles = userRoles.Select(z => new UserRoleDTO
            {
                Name = z
            });

            return userDTO;
        }
    }
}