using Gemini.Framework.Commands;

namespace Gemini.Modules.Output.Commands
{
    // I'm thinking we should reduce the amount of ceremony involved to get a Tool to show
    // up in the View menu. Take a look at the alternate way to Import and then inject all
    // loaded ITool's into the menu within ShellViewModel
    //
    //[CommandDefinition]
    //public class ViewOutputCommandDefinition : CommandDefinition
    //{
    //    public const string CommandName = "View.Output";

    //    public override string Name
    //    {
    //        get { return CommandName; }
    //    }

    //    public override string Text
    //    {
    //        get { return "_Output"; }
    //    }

    //    public override string ToolTip
    //    {
    //        get { return "Output"; }
    //    }
    //}
}