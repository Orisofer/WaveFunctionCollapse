using UnityEngine;

namespace WFC
{
    [CreateAssetMenu(fileName = "Tile", menuName = "Wave Function Collapse/Tile")]
    public class Tile : ScriptableObject
    {
        [SerializeField] private Sprite TileSprite;

        [SerializeField] private EdgeType EdgeUp;
        [SerializeField] private EdgeType EdgeDown;
        [SerializeField] private EdgeType EdgeLeft;
        [SerializeField] private EdgeType EdgeRight;

        [SerializeField] private int m_Weight = 1;
        
        public Sprite Sprite => TileSprite;
        public int Weight => m_Weight;

        public EdgeType GetTypeInDirection(CellDirection direction)
        {
            EdgeType result = direction switch
            {
                CellDirection.Up => EdgeUp,
                CellDirection.Down => EdgeDown,
                CellDirection.Left => EdgeLeft,
                CellDirection.Right => EdgeRight,
                _ => default
            };

            return result;
        }
    }
}