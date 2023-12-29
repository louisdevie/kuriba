using Kuriba.Core.Serialization.Converters;
using System.Reflection;

namespace Kuriba.Core.Serialization
{
    internal class MessageField
    {
        private readonly PropertyInfo propertyInfo;
        private readonly Converter converter;

        public MessageField(PropertyInfo propertyInfo)
        {
            this.propertyInfo = propertyInfo;
            this.converter = ConverterFactory.Default.GetConverterFor(this.propertyInfo.PropertyType);
        }

        public void WriteValueFromMessage(object message, IMessageWriter output, IReferenceTracker tracker)
        {
            object? value = this.propertyInfo.GetValue(message);

            if (this.propertyInfo.PropertyType.IsByRef)
            {
                tracker.TrackObject(value);
            }

            this.converter.Write(value, output);
        }

        public void ReadValueIntoMessage(object message, IMessageReader input)
        {
            object? value = this.converter.Read(this.propertyInfo.PropertyType, input);
            
            this.propertyInfo.SetValue(message, value);
        }
    }
}