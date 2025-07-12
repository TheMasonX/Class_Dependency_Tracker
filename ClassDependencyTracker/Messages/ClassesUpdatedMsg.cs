using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDependencyTracker.Messages;

public enum UpdateType
{
    None = 0,
    Added = 1,
    Removed = 2,
    Replaced = 3,
}

public record ClassesUpdatedMsg(UpdateType ClassUpdate, UpdateType RequirementUpdate)
{
}
