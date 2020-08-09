using System;

namespace Web.Areas.Admin.Application.Users.Exceptions
{
    public class UserEmailAddressIsAlreadyTakenException : Exception
    {
        public UserEmailAddressIsAlreadyTakenException() : base(
            "Current email address is already taken by another user")
        {
            
        }
    }
}