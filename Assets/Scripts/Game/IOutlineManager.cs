using System.Collections.Generic;

namespace Game
{
    public interface IOutlineManager
    {
        void SetShines(List<Outline> outlines);

        void Disable();
    }
}