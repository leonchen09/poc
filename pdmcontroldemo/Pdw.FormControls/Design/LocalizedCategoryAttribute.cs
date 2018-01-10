using System;
using System.ComponentModel;
using System.Resources;
using Pdw.FormControls.Properties;

namespace Pdw.FormControls.Design
{
    public sealed class LocalizedCategoryAttribute : CategoryAttribute
    {
        public LocalizedCategoryAttribute(string resourceName) : this(resourceName, typeof(Resources)) { }

        public LocalizedCategoryAttribute(string resourceName, Type resourceType) : base(resourceName)
        {
            ResourceType = resourceType;
        }

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

        protected override string GetLocalizedString(string value)
        {
            return ResourceManager.GetString(value);
        }
    }
}
