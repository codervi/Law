namespace LawFirm.Web.Helpers;

public static class TextHelper
{
    /// <summary>"Eleanor Calder" -> "EC". Used for avatar monograms when a lawyer has no photo.</summary>
    public static string Initials(string fullName) =>
        string.Concat(fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(p => char.ToUpperInvariant(p[0])));
}
