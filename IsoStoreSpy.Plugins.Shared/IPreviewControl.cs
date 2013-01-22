using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoStoreSpy.Plugins.Shared;

namespace IsoStoreSpy.Plugins.Shared
{
    public interface IPreviewControl
    {
        /// <summary>
        /// Plugin
        /// </summary>

        IPreviewPlugin Plugin
        {
            get;
            set;
        }
    }
}
