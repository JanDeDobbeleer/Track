namespace Settings
{
    public class AppSettings
    {
        public string Title { get; set; }
        public string TitlePanelHeader { get; set; }
        public string TitlePanelSubscript { get; set; }
        public string AboutTitle { get; set; }
        public string BlogUri { get; set; }
        public string ExternalContent { get; set; }
        public string ExternalUri { get; set; }
        public string Email { get; set; }
        public string EmailSubject { get; set; }

        private AppSettings _default;

        public AppSettings Default
        {
            get { return _default; }
            set
            {
                if (_default == null)
                    _default = value;
            }
        }
    }
}
