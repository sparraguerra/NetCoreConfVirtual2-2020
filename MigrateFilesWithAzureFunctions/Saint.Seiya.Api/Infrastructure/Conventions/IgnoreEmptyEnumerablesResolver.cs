using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Reflection;

namespace Saint.Seiya.Api.Infrastructure.Conventions
{
    /// <summary>
    /// IgnoreEmptyEnumerablesResolver
    /// </summary>
    public class IgnoreEmptyEnumerablesResolver : DefaultContractResolver
    {
        /// <summary>
        /// CreateProperty
        /// </summary>
        /// <param name="member"></param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyType != typeof(string) &&
                typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                property.ShouldSerialize = instance =>
                {
                    IEnumerable enumerable = null;

                    // this value could be in a public field or public property
                    if (member.MemberType == MemberTypes.Property)
                    {

                        enumerable = instance
                                        .GetType()
                                        .GetProperty(member.Name)
                                        .GetValue(instance, null) as IEnumerable; 
                    }
                    if (member.MemberType == MemberTypes.Field)
                    { 
                        enumerable = instance
                                        .GetType()
                                        .GetField(member.Name)
                                        .GetValue(instance) as IEnumerable; 
                    }

                    if (enumerable != null)
                    {
                        // check to see if there is at least one item in the Enumerable
                        return enumerable.GetEnumerator().MoveNext();
                    }
                    // if the list is null, we defer the decision to NullValueHandling
                    return true;
                };
            }

            return property;
        }
    }
}
