
namespace Pdw.Core
{
    public enum BookmarkType:int
    {
        StartForeach = 1,
        EndForeach = 2,
        StartIf = 3,
        EndIf = 4,
        Select = 5,
        Image = 6,
        Comment,
        None,
        PdeTag,
        PdeChart,
    }
}
