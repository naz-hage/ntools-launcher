namespace Launcher
{
    public class Parameters
    {
        public string WorkingDir { get; set; }
        public string FileName { get; set; }
        public string Arguments { get; set; }
        public bool RedirectStandardOutput { get; set; } = false;
        public bool Verbose { get; set; } = false;
        public bool UseShellExecute { get; set; } = false;
    }
}