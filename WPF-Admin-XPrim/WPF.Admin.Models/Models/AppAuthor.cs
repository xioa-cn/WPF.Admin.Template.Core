
namespace WPF.Admin.Models.Models
{
    public class AppAuthor
    {
        public static readonly string Author = "XIOA";

        private static string _version = "1.0.0.0";

        public static string Version
        {
            get => _version;
            set
            {
                _version = value;
                //通知
                OnVersionChanged?.Invoke();
            }
        }

        public static Action OnVersionChanged { get; set; }
    }
}
