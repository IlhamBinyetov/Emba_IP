namespace Emba_IP.ViewModels.User
{
    public class UserEditViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public List<string> AllRoles { get; set; }
        public string SelectedRole { get; set; }
    }
}
