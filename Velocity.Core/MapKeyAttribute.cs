using System;
using Velocity.Core.Extensions;

namespace Velocity.Core
{
    public class MapKeyAttribute : Attribute
    {
        public MapKeyAttribute(string propertyName, string targetId = "Id")
        {
            PropertyName = propertyName;
            TargetId = targetId;
        }

        public string PropertyName { get; }
        public string TargetId { get; }

        public bool SetProperty(object objToMake, object targetToRead)
        {
            try
            {
                var id = targetToRead.GetType().GetProperty(TargetId).GetValue(targetToRead);

                if (id.IsNullOrDefault())
                    return false;

                objToMake.SetValue(PropertyName, id);

                return true;
            }
            catch (Exception e)
            {
                throw new FactoryMakerException("Failed to read or set id on map-key", e);
            }
        }
    }
}