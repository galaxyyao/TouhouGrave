using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace V2DRuntime
{
    public interface IResourceManager
    {
        DDW.V2D.V2DContent LoadV2DContent(string assetName);
    }

    public static class ResourceManager
    {
        private static IResourceManager m_manager = null;

        public static IResourceManager Instance
        {
            get { return m_manager; }
            set { m_manager = value; }
        }
    }
}
