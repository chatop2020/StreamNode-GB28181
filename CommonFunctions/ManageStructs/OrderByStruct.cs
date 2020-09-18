using System;

namespace CommonFunctions.ManageStructs
{
    [Serializable]
    public enum OrderByDir
    {
        ASC,
        DESC
    }

    [Serializable]
    public class OrderByStruct
    {
        private string? _fieldName = null!;
        private OrderByDir? _orderByDir;

        public string? FieldName
        {
            get => _fieldName;
            set => _fieldName = value ?? throw new ArgumentNullException(nameof(value));
        }

        public OrderByDir? OrderByDir
        {
            get => _orderByDir;
            set => _orderByDir = value;
        }
    }
}