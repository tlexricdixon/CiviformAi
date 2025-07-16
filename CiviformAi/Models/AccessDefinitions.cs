namespace CiviformAi.Models;

public class FormDefinition
{
    public string FormName { get; set; }
    public string RecordSource { get; set; }
    public List<ControlDefinition> Controls { get; set; }
}

public class ControlDefinition
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string ControlSource { get; set; }
    public string Label { get; set; }
    public int? Left { get; set; }
    public int? Top { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
}
