using Bogus;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Velocity.Core
{
    public abstract class Factory<TObject>
        where TObject : class
    {
        protected readonly Faker Faker = new Faker();

        public virtual TObject Make(bool createRequested = false)
        {
            try
            {
                var obj = ObjectFactory.Create<TObject>();

                foreach (var property in GetType().GetProperties())
                {
                    var value = property.GetValue(this);

                    var mapKey = property.GetCustomAttribute<MapKeyAttribute>();

                    if (mapKey == null)
                    {
                        obj.SetValue(property.Name, value);
                        continue;
                    }
                    else
                    {
                        var isSet = mapKey.SetProperty(obj, value);

                        if (!createRequested || !isSet)
                            obj.SetValue(property.Name, value);
                    }
                }

                return obj;
            }
            catch (Exception e)
            {
                throw new FactoryMakerException(e.ToString(), e);
            }
        }

        public virtual List<TObject> MakeRange(int number, bool createRequested = false)
        {
            var list = new List<TObject>();
            for (var i = 0; i < number; i++)
                list.Add(Make());

            return list;
        }
    }
}
