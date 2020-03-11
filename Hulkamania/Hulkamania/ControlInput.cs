namespace Brandeis.AGSOL.Hulkamania
{
    /// <summary>
    /// Input from joystick, forcing function, etc.
    /// </summary>
    public struct ControlInput
    {
        public readonly double x;
        public readonly double y;

        public readonly bool trigger;
        public readonly bool blanked;

        public ControlInput(double xPos, double yPos, bool triggerButton, bool blankedInput)
        {
            x = xPos;
            y = yPos;
            trigger = triggerButton;
            blanked = blankedInput;
        }
    }
}
