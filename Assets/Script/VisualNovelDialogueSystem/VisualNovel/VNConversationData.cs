using System.Collections.Generic;

namespace VisualNovel
{
    [System.Serializable]
    public class VNConversationData 
    {
        public List<string> conversation = new List<string>();
        public int progress;
    }
}