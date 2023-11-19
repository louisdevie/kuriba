using Kuriba.Core.Serialization.Converters;
using System.Reflection;

namespace Kuriba.Core.Serialization
{
    internal class MessageField
    {
        private PropertyInfo propertyInfo;
        private Converter converter;

        public MessageField(PropertyInfo propertyInfo)
        {
            this.propertyInfo = propertyInfo;
            this.converter = ConverterFactory.Default.GetConverterFor(this.propertyInfo.PropertyType);
        }

        public void WriteValueFromMessage(object message, IMessageWriter output)
        {
            this.converter.Write(
                this.propertyInfo.GetValue(message),
                output
            );
        }
    }

}
