namespace WPF.Admin.Themes.Controls
{
    public class ChangeThemeEvent
    {
        public ChangeThemeEvent(string key)
        {
            Key = key;
        }

        public string Key { get; set; }


        public ChangeThemeEvent()
        {
            
        }
    }
}