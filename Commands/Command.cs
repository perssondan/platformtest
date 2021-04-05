using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uwpKarate.Actors;

namespace uwpKarate.Commands
{
    public abstract class Command
    {
        public abstract void Execute(GameObject actor);
    }
}
