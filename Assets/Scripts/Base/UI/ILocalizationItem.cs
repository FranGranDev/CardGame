
namespace UI
{
    interface ILocalizationItem
    {
        public string FieldName { get; }
        public void SetText(string text);
    }
}
