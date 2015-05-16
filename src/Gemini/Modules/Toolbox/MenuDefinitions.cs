using System.ComponentModel.Composition;
using Gemini.Framework.Menus;
using Gemini.Modules.Toolbox.Commands;

namespace Gemini.Modules.Toolbox
{
    // We no longer need this extra boiler plate as Tool viewmodels can now define how they get injected into the view menu
    //public static class MenuDefinitions
    //{
    //    [Export]
    //    public static MenuItemDefinition ViewToolboxMenuItem = new CommandMenuItemDefinition<ViewToolboxCommandDefinition>(
    //        MainMenu.MenuDefinitions.ViewToolsMenuGroup, 4);
    //}
}