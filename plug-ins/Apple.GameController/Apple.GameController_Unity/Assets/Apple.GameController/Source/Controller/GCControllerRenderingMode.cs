namespace Apple.GameController.Controller
{
    public enum GCControllerRenderingMode : int
    {
        /// <summary>
        /// Uses the default symbol rendering based on existing drawing context.
        /// </summary>
        Automatic = 0,
        /// <summary>
        /// If the SFSymbols supports full color, then this wil utilize the original colors.
        /// </summary>
        AlwaysOriginal = 1,
        /// <summary>
        /// Always uses the template (outline) glyph for the SF Symbol.
        /// </summary>
        AlwaysTemplate = 2
    }
}