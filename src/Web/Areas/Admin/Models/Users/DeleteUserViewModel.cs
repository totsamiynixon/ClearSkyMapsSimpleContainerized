using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.Models.Users
{
    public class DeleteUserViewModel
    {

        public DeleteUserViewModel(DeleteUserModel model)
        {
            Model = model;
        }
        public DeleteUserModel Model { get; }
    }
}