<Query Kind="Statements">
  <Namespace>System.ComponentModel</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>

var valueTypes = Enum.GetValues<TypeCode>().Select(code => Type.GetType($"System.{code}")).Where(x => x.IsValueType);

foreach (var from in valueTypes.Select(type => new { Type = type, MaxValue = type.GetField("MaxValue", BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField)?.GetValue(null), MinValue = type.GetField("MinValue", BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField)?.GetValue(null) }))
{
	foreach (var value in new object[] { from.MaxValue, from.MinValue })
	{
		var fromInstance = value ?? Activator.CreateInstance(from.Type);
		var fromConverter = TypeDescriptor.GetConverter(from.Type);
		
		if(fromConverter is System.ComponentModel.DateTimeConverter)
			fromConverter = new DateTimeConverter();
		 	
						
		foreach (var toType in valueTypes.Where(toType  => toType != from.Type))
		{
			var toConverter = TypeDescriptor.GetConverter(toType);

			if (toConverter is System.ComponentModel.DateTimeConverter)
				toConverter = new DateTimeConverter();

			object? converted = null;

			try
			{
				if (fromConverter.CanConvertTo(toType))
				{
					converted = fromConverter.ConvertTo(fromInstance, toType);
				}
				else if (toConverter.CanConvertFrom(from.Type))
				{
					converted = toConverter.ConvertFrom(fromInstance);
				}
				else
				{
					$"Could not convert {(fromInstance is null ? "null" : fromInstance)}<{from.Type}> to <{toType}>".Dump();
				}
			}
			catch (InvalidCastException e) 
			{
				$"Could not convert {fromInstance}<{from.Type}> to <{toType}>".Dump();
			}
			catch (OverflowException e)
			{
				$"Could not convert {fromInstance}<{from.Type}> to <{toType}>".Dump();
			}
			
			if (converted is not null)
			{
				$"converted {fromInstance}<{from.Type}> to {converted}<{toType}>".Dump();
			}
		}
	}
};