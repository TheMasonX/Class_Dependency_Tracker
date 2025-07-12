using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDependencyTracker.Messages;

public enum ClassUpdateType
{
    Added = 0,
    Removed = 1,
}

public record ClassesUpdatedMsg(ClassUpdateType UpdateType)
{
}
