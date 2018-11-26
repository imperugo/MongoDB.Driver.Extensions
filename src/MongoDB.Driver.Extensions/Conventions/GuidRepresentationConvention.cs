using System;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace MongoDB.Driver.Extensions.Conventions
{
	/// <summary>
	/// A convention that allows you to set the serialization representation of guid to a simple string
	/// </summary>
	public class GuidRepresentationConvention : ConventionBase, IMemberMapConvention
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="GuidRepresentationConvention" /> class.
        /// </summary>
        /// <param name="representation">The serialization representation. string is used to detect representation
        /// from the Guid itself.</param>
        public GuidRepresentationConvention(BsonType representation)
		{
			EnsureRepresentationIsValidForGuid(representation);
			Representation = representation;
		}

        /// <summary>
        /// Gets the representation.
        /// </summary>
        public BsonType Representation { get; }

        /// <summary>
        /// Applies a modification to the member map.
        /// </summary>
        /// <param name="memberMap">The member map.</param>
        public void Apply(BsonMemberMap memberMap)
		{
			var memberType = memberMap.MemberType;
			var memberTypeInfo = memberType.GetTypeInfo();

			if (memberTypeInfo == typeof(Guid))
			{
				var serializer = memberMap.GetSerializer();
				if (serializer is IRepresentationConfigurable representationConfigurableSerializer)
				{
					var reconfiguredSerializer = representationConfigurableSerializer.WithRepresentation(Representation);
					memberMap.SetSerializer(reconfiguredSerializer);
				}
				return;
			}

			if (IsNullableGuid(memberType))
			{
				var serializer = memberMap.GetSerializer();
				if (serializer is IChildSerializerConfigurable childSerializerConfigurableSerializer)
				{
					var childSerializer = childSerializerConfigurableSerializer.ChildSerializer;
					if (childSerializer is IRepresentationConfigurable representationConfigurableChildSerializer)
					{
						var reconfiguredChildSerializer = representationConfigurableChildSerializer.WithRepresentation(Representation);
						var reconfiguredSerializer = childSerializerConfigurableSerializer.WithChildSerializer(reconfiguredChildSerializer);
						memberMap.SetSerializer(reconfiguredSerializer);
					}
				}
			}
		}

		private bool IsNullableGuid(Type type)
		{
			return
				type.GetTypeInfo().IsGenericType 
					&& type.GetGenericTypeDefinition() == typeof(Nullable<>) 
					&& Nullable.GetUnderlyingType(type).GetTypeInfo() == typeof(Guid);
		}

		private void EnsureRepresentationIsValidForGuid(BsonType bsonType)
		{
			if (bsonType == BsonType.String)
			{
				return;
			}
			
			throw new ArgumentException("Guid can only be represented as String.", nameof(bsonType));
		}
	}
}