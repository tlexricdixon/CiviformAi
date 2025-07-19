using Entities;

namespace CiviformAi.Models;

public class FormDefinition
{
    public string FormName { get; set; }
    public string RecordSource { get; set; }
    public List<ControlDefinition> Controls { get; set; }
}

