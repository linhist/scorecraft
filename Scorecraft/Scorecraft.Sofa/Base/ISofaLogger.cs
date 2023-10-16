namespace Scorecraft.Sofa
{
    public interface ISofaLogger
    {
        bool AllowSave { get; }

        string Errors { get; }

        void AddError(string message, string source);

        bool SaveContent(string content, string file);

        bool SaveResource(string url, string file);
    }
}