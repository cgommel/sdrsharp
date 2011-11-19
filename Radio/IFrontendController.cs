namespace SDRSharp.Radio
{
    public interface IFrontendController
    {
        void Open();
        void Close();
        bool IsOpen { get; }
        int Frequency { get; set; }
    }
}
