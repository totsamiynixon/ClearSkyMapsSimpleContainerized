using System;

namespace Web.Areas.Admin.Application.Users.Exceptions
{
    public class UserEmailAddressIsAlreadyTakenException : Exception
    {
        public UserEmailAddressIsAlreadyTakenException(string email) : base(
            $"Email address: {email} is already taken by another user")
        {
            
        }
    }
}