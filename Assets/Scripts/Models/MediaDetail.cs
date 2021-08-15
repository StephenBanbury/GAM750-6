using System;
using Assets.Scripts.Enums;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class MediaDetail
    {
        public int Id;
        public string Title;
        public MediaType MediaType;
        public Source Source;
        public string Url;
        public string Filename;
        public string LocalPath;
    }
}
