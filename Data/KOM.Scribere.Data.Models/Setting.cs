namespace KOM.Scribere.Data.Models;

using KOM.Scribere.Data.Common.Models;

public class Setting : BaseDeletableModel<int>
{
    public string Name { get; set; }

    public string Value { get; set; }
}