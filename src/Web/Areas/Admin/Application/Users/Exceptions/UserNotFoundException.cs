using System;

namespace Web.Areas.Admin.Application.Users.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string userId) : base($"User with: {userId} not found")
        {
            
        }
    }
}