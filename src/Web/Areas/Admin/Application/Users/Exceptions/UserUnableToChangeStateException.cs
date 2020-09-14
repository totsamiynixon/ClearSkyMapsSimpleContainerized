using System;

namespace Web.Areas.Admin.Application.Users.Exceptions
{
    public class UserUnableToChangeStateException : Exception
    {
        public UserUnableToChangeStateException(string reason) : base(reason)
        {
            
        }
    }
}