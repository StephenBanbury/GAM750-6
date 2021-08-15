using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class ScreenPortalBufferState
    {
        public int ScreenId { get; set; }
        public bool IsPortal { get; set; }
        public int DestinationSceneId { get; set; }
    }

}