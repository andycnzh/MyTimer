using MyTimer.Resources;

namespace MyTimer
{
    /// <summary>
    /// 提供对字符串资源的访问权。
    /// </summary>
    public class LocalizedStrings
    {
        public LocalizedStrings()
        {
        }

        private static AppResources _localizedResources = new AppResources();

        public AppResources LocalizedResources { get { return _localizedResources; } }
        
    }
}