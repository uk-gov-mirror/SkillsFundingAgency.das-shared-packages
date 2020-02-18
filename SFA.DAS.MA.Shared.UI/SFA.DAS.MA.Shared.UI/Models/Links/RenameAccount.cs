﻿namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class RenameAccount : Link
    {
        public RenameAccount(string href) : base(href)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" role=\"menuitem\" class=\"sub-menu-item\">Rename account</a>";
        }
    }
}
