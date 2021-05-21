using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Commands
{
    public abstract class Command
    {
        public abstract void Execute(GameObject actor);
    }
}
