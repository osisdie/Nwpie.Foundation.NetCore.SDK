namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class Auth_OptionCollection : OptionBase
    {
        public string AdminMails { get; set; }
        public string AdminAccountId { get; set; }
        public Auth_Option Token { get; set; }
    }
}
