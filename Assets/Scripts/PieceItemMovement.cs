using DG.Tweening;
using UnityEngine;

public class PieceItemMovement : MonoBehaviour
{
    private Tile _thisTile;
    private Tile _clickedTile;
    private Tile _targetTile;

    private BoardMovement _boardMovement;
    private BoardCreator _boardCreator;

    private PieceItem _pieceItem;


    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _boardCreator = BoardManager.Instance.boardCreator;
        _boardMovement = BoardManager.Instance.boardMovement;
        _pieceItem = GetComponent<PieceItem>();
    }

    public void MoveAction(int x, int y, float time)
    {
        if (_boardMovement.isMoving) return;
        Move(new Vector3(x, y, -1), time);
    }

  private void Move(Vector3 destination, float timeToMove)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOMove(destination, timeToMove).SetEase(Ease.InOutQuad));
            sequence.Join(transform.DOScale(new Vector3(1.1f, 1.1f, 1f), timeToMove / 2).SetEase(Ease.OutBack));
            sequence.OnStart(() => _boardMovement.isMoving = true);
            sequence.OnComplete(() =>
            {
                _boardCreator.PlacementOfItem(_pieceItem, (int)destination.x, (int)destination.y, _pieceItem.poolItemType);
                _boardMovement.isMoving = false;
                transform.DOScale(Vector3.one, timeToMove / 2).SetEase(Ease.InBack);
            });
            sequence.Play();
        }
}
