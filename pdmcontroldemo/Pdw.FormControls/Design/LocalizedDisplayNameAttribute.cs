using System;
using System.ComponentModel;
using System.Resources;
using Pdw.FormControls.Properties;

namespace Pdw.FormControls.Design
{
    public sealed class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        public LocalizedDisplayNameAttribute(string resourceName) : this(resourceName, typeof(Resources)) { }

        public LocalizedDisplayNameAttribute(string resourceName, Type resourceType)
        {
            ResourceName = resourceName;
            ResourceType = resourceType;
        }

        public string ResourceName { get; private set; }

        public Type ResourceType { get; private set; }

        private ResourceManager _resourceManager;

        protected ResourceManager ResourceManager
        {
            get
            {
                if (_resourceManager == null)
                {
                    _resourceManager = new ResourceManager(ResourceType);
                }

                return _resourceManager;
            }
        }

        public override string DisplayName
        {
            get
            {
                return ResourceManager.GetString(ResourceName);
            }
        }
    }
}
