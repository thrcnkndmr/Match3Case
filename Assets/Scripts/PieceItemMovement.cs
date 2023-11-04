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
        var scaleUp = new Vector3(1.3f, 1.3f, 1.3f);
        var originalScale = transform.localScale;
        var moveSequence = DOTween.Sequence();

        moveSequence.Append(transform.DOMove(destination, timeToMove).SetEase(Ease.Linear))
            .Join(transform.DOScale(scaleUp, timeToMove / 2))
            .Append(transform.DOScale(originalScale, timeToMove / 2))
            .OnStart(() => _boardMovement.isMoving = true)
            .OnComplete(() =>
            {
                _boardCreator.PlacementOfItem(_pieceItem, (int)destination.x, (int)destination.y);
                _boardMovement.isMoving = false;
            });

        moveSequence.Play();
    }
}