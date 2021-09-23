
namespace Ganymed.UISystem
{
    public readonly struct ConfirmCancelConfiguration
    {
        public readonly string labelText;
        public readonly string confirmButtonLabelText;
        public readonly string cancelButtonLabelText;

        public ConfirmCancelConfiguration(string labelText, string confirmButtonLabelText = null, string cancelButtonLabelText = null)
        {
            this.labelText = labelText;
            this.confirmButtonLabelText = confirmButtonLabelText;
            this.cancelButtonLabelText = cancelButtonLabelText;
        }
    }
}