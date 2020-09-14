using System;

namespace Web.Areas.Admin.Application.Users.Exceptions
{
    public class UserUnableToCreateException : Exception
    {
        public UserUnableToCreateException(string reason) : base($"Unable to create user. Reason: {reason}")
        {
            
        }
    }
}