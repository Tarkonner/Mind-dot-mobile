using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LevelMetadata 
{
    public static int[] PiecesMetadataCalculation(List<PieceElement> piecesData, InteractiveGrid grid)
    {
        int[] result = new int[piecesData.Count];
        for (int i = 0; i < piecesData.Count; i++)
            result[i] = CountValidPlacements(piecesData[i], grid);
        return result;
    }

    public static int CountValidPlacements(PieceElement piece, InteractiveGrid grid)
    {
        // Normalize original piece shape to top-left corner
        List<Vector2Int> originalDotPositions = piece.GetDotCellPositions();
        for (int i = 0; i < originalDotPositions.Count; i++)
        {
            originalDotPositions[i] -= piece.gridData.gridPosRef;
        }

        int validPlacementCount = 0;
        Vector2 gridSize = grid.GridSize();

        // Try all 4 90-degree rotations
        for (int rotation = 0; rotation < 4; rotation++)
        {
            List<Vector2Int> rotatedDots = RotateDots(originalDotPositions, rotation);

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    bool canPlace = true;

                    for (int i = 0; i < rotatedDots.Count; i++)
                    {
                        Vector2Int cal = rotatedDots[i] + new Vector2Int(x, y);
                        if (cal.x < 0 || cal.x >= gridSize.x || cal.y < 0 || cal.y >= gridSize.y)
                        {
                            canPlace = false;
                            break;
                        }

                        CellElement cell = grid.cells[cal.y * InteractiveGrid.gridSize + cal.x];
                        if (cell.cellData.turnedOff)
                        {
                            canPlace = false;
                            break;
                        }
                        if (!cell.cellData.partOfPiece && cell.cellData.holding != null)
                        {
                            canPlace = false;
                            break;
                        }
                    }

                    if (canPlace)
                        validPlacementCount++;
                }
            }
        }

        return validPlacementCount;
    }

    private static List<Vector2Int> RotateDots(List<Vector2Int> dots, int rotation)
    {
        List<Vector2Int> rotated = new List<Vector2Int>();

        foreach (var dot in dots)
        {
            Vector2Int r;

            switch (rotation)
            {
                case 0: // 0°
                    r = new Vector2Int(dot.x, dot.y);
                    break;
                case 1: // 90°
                    r = new Vector2Int(-dot.y, dot.x);
                    break;
                case 2: // 180°
                    r = new Vector2Int(-dot.x, -dot.y);
                    break;
                case 3: // 270°
                    r = new Vector2Int(dot.y, -dot.x);
                    break;
                default:
                    r = dot;
                    break;
            }

            rotated.Add(r);
        }

        // Normalize to top-left (0,0) based position
        int minX = rotated.Min(p => p.x);
        int minY = rotated.Min(p => p.y);
        for (int i = 0; i < rotated.Count; i++)
        {
            rotated[i] -= new Vector2Int(minX, minY);
        }

        return rotated;
    }
}
