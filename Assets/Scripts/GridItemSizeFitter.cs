using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class GridItemSizeSetter : MonoBehaviour {

    public int rowCount = 1;
    public int columnCount = 1;

    public float CellHeight {
        get {
            return (int)((rectTransform.sizeDelta.y - (gridLayout.padding.top + gridLayout.padding.bottom)
                - gridLayout.spacing.y * (rowCount - 1)) / rowCount);
        }
    }

    public int CellWidth {
        get {
            return (int)((rectTransform.sizeDelta.x - (gridLayout.padding.left + gridLayout.padding.right)
                - gridLayout.spacing.x * (columnCount - 1)) / columnCount);
        }
    }

    private RectTransform rectTransform;
    private GridLayoutGroup gridLayout;

    void Start() {
        rectTransform = GetComponent<RectTransform>();
        gridLayout = GetComponent<GridLayoutGroup>();
        gameObject.ObserveEveryValueChanged(_ => rectTransform.sizeDelta).Subscribe(_ => UpdateCellSize());
        gameObject.ObserveEveryValueChanged(_ => gridLayout.spacing).Subscribe(_ => UpdateCellSize());
        gameObject.ObserveEveryValueChanged(_ => gridLayout.padding.left).Subscribe(_ => UpdateCellSize());
        gameObject.ObserveEveryValueChanged(_ => gridLayout.padding.right).Subscribe(_ => UpdateCellSize());
        gameObject.ObserveEveryValueChanged(_ => gridLayout.padding.top).Subscribe(_ => UpdateCellSize());
        gameObject.ObserveEveryValueChanged(_ => gridLayout.padding.bottom).Subscribe(_ => UpdateCellSize());
        gameObject.ObserveEveryValueChanged(_ => rowCount).Subscribe(_ => UpdateCellSize());
    }

    private void UpdateCellSize() {
        gridLayout.cellSize = new Vector2(CellWidth, CellHeight);
    }

}