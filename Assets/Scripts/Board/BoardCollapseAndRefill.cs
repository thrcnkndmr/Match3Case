using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardCollapseAndRefill : MonoBehaviour
{
    private BoardMatchFinding _boardMatchFinding;
    private BoardMovement _boardMovement;
    private BoardCreator _boardCreator;
    private GameManager _gameManager;
    private Pool _pool;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _boardMatchFinding = BoardManager.Instance.boardMatchFinding;
        _boardMovement = BoardManager.Instance.boardMovement;
        _boardCreator = BoardManager.Instance.boardCreator;
        _gameManager = GameManager.Instance;
        _pool = Pool.Instance;
    }

    private IEnumerable<PieceItem> CollapseColumn(int column, float collapseTime = 0.1f)
    {
        var movingPieces = new HashSet<PieceItem>();
        for (var i = 0; i < _boardCreator.height - 1; i++)
        {
            if (_boardCreator.PieceItems[column, i] != null) continue;
            for (var j = i + 1; j < _boardCreator.height; j++)
            {
                if (_boardCreator.PieceItems[column, j] == null) continue;

                var piece = _boardCreator.PieceItems[column, j];
                _boardCreator.PieceItems[column, i] = piece;
                _boardCreator.PieceItems[column, j] = null;

                movingPieces.Add(piece);
                piece.pieceItemMovement.MoveAction(column, i, collapseTime * (j - i));
                break;
            }
        }

        return movingPieces;
    }

    private IEnumerable<PieceItem> CollapseColumn(IEnumerable<PieceItem> gamePieces)
    {
        if (_gameManager.gameState == GameManager.GameState.SuccessFail)
            return Enumerable.Empty<PieceItem>();
        var movingPieces = new List<PieceItem>();
        var columnsToCollapse = GetColumns(gamePieces);
        return columnsToCollapse.Aggregate(movingPieces,
            (current, column) => current.Union(CollapseColumn(column)).ToList());
    }

    private IEnumerable<int> GetColumns(IEnumerable<PieceItem> gamePieces)
    {
        if (_gameManager.gameState == GameManager.GameState.SuccessFail)
            return Enumerable.Empty<int>();

        var columns = new List<int>();

        foreach (var piece in gamePieces.Where(piece => !columns.Contains(piece.rowIndex)))
        {
            columns.Add(piece.rowIndex);
        }

        return columns;
    }

    public void ClearAndRefillBoard(List<PieceItem> gamePieces)
    {
        if (_gameManager.gameState == GameManager.GameState.SuccessFail) return;
        StartCoroutine(ClearAndRefillBoardRoutine(gamePieces));
    }

    private IEnumerator ClearAndRefillBoardRoutine(List<PieceItem> gamePieces)
    {
        if (_gameManager.gameState == GameManager.GameState.SuccessFail) yield break;
        _boardMovement.canPlayerTouch = false;
        var matches = gamePieces;

        do
        {
            yield return StartCoroutine(ClearAndCollapseRoutine(matches));
            yield return null;

            RefillBoard();
            matches = _boardMatchFinding.FindAllMatches();

            yield return new WaitForSeconds(0.5f);
        } while (matches.Count != 0);

        _boardMovement.canPlayerTouch = true;
    }

    private IEnumerator ClearAndCollapseRoutine(List<PieceItem> gamePieces)
    {
        if (_gameManager.gameState == GameManager.GameState.SuccessFail) yield break;
        _boardMatchFinding.HighlightPieces(gamePieces);
        yield return new WaitForSeconds(0.4f);

        foreach (var piece in gamePieces)
        {
            _boardMatchFinding.ClearPieceAt(piece.rowIndex, piece.columnIndex);
        }

        yield return new WaitForSeconds(0.4f);

        var movingPieces = CollapseColumn(gamePieces).ToList();
        yield return new WaitUntil(() => IsCollapsed(movingPieces));
        _boardMatchFinding.ClearAllHighlights();
        var matches = _boardMatchFinding.FindMatchesAt(movingPieces).ToList();
        if (matches.Count > 0)
        {
            yield return StartCoroutine(ClearAndCollapseRoutine(matches));
        }
    }

    private static bool IsCollapsed(IEnumerable<PieceItem> gamePieces)
    {
        return gamePieces.All(piece =>
            piece == null || !(Mathf.Abs(piece.transform.position.y - piece.columnIndex) > 0.001f));
    }

    private void FillColumnFromAbove(int column)
    {
        if (_gameManager.gameState == GameManager.GameState.SuccessFail) return;
        for (var y = _boardCreator.height - 1; y >= 0; y--)
        {
            if (_boardCreator.PieceItems[column, y] != null) continue;
            var randomItemType = _boardCreator.GetRandomItem();
            var spawnPosition = new Vector3(column, _boardCreator.height + y, 0);
            _pool.SpawnObject(spawnPosition, randomItemType, null, Quaternion.identity)
                .TryGetComponent(out PieceItem pieceItem);
            _boardCreator.PlacementOfItem(pieceItem, column, y, randomItemType);
            pieceItem.pieceItemMovement.MoveAction(column, y, 0.3f);
        }
    }

    private void RefillBoard()
    {
        if (_gameManager.gameState == GameManager.GameState.SuccessFail) return;
        for (var column = 0; column < _boardCreator.width; column++)
        {
            FillColumnFromAbove(column);
        }
    }
}