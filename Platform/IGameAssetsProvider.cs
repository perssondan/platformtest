using System.Threading.Tasks;
using uwpPlatformer.Models;

namespace uwpPlatformer.Platform
{
    public interface IGameAssetsProvider
    {
        Task LoadAssetsAsync();

        Map Map { get; }

        bool TryGetTileSet(int id, out TileSet tileSet);

        bool TryGetTileSet(string name, out TileSet tileSet);
    }
}
