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

        public virtual TObject Make()
        {
            try
            {
                var obj = ObjectFactory.Create<TObject>();

                foreach (var property in GetType().GetRuntimeProperties())
                {
                    obj.SetValue(property.Name, property.GetValue(this));
                }

                return obj;
            }
            catch (Exception e)
            {
                throw new FactoryMakerException(e.ToString(), e);
            }
        }

        public virtual List<TObject> MakeRange(int number)
        {
            var list = new List<TObject>();
            for (var i = 0; i < number; i++)
                list.Add(Make());

            return list;
        }
    }
}
