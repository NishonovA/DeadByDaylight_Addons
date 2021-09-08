using System;
using System.ComponentModel;

namespace DeadByDaylight_Addons
{
    enum KindSortEnum
    {
        [Description("По порядку")]
        Standart,

        [Description("По рейтингу")]
        Rate
    }

    internal class CustomComboboxItem
    {
        public KindSortEnum Order { get; set; }

        public override string ToString()
        {
            var field = Order.GetType().GetField(Order.ToString());

            DescriptionAttribute attribute
                    = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                        as DescriptionAttribute;

            return attribute == null ? Order.ToString() : attribute.Description;
        }
    }
}
