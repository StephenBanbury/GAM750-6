using Assets.Scripts.Enums;

namespace Assets.Scripts.Models
{
    public class ScreenActionModel
    {
        public int ScreenId { get; set; }
        public ScreenAction NextAction { get; set; }
    }
}
