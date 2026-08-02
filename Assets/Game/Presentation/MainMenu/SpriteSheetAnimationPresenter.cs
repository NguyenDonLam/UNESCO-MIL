using System;
using UnityEngine;

namespace Game.Presentation.MainMenu
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class SpriteSheetAnimationPresenter : MonoBehaviour
    {
        [SerializeField] private Texture2D spriteSheet;
        [SerializeField, Min(1)] private int columns = 3;
        [SerializeField, Min(1)] private int rows = 2;
        [SerializeField, Min(0.1f)] private float framesPerSecond = 6f;
        [SerializeField] private bool fitToCameraWidth = true;
        [SerializeField, Min(0.01f)] private float scaleMultiplier = 1f;
        [SerializeField] private Vector2 positionOffset;

        private SpriteRenderer _spriteRenderer;
        private Sprite[] _frames = Array.Empty<Sprite>();
        private float _secondsPerFrame;
        private float _elapsed;
        private int _frameIndex;

        private void Awake()
        {
            if (spriteSheet == null)
                throw new InvalidOperationException(name + " requires a sprite sheet.");
            if (columns <= 0 || rows <= 0)
                throw new ArgumentOutOfRangeException(nameof(columns), "Sprite-sheet dimensions must be positive.");
            if (spriteSheet.width % columns != 0 || spriteSheet.height % rows != 0)
                throw new InvalidOperationException("The sprite sheet must divide evenly into its frame grid.");
            if (framesPerSecond <= 0f)
                throw new ArgumentOutOfRangeException(nameof(framesPerSecond));

            _spriteRenderer = GetComponent<SpriteRenderer>();
            _secondsPerFrame = 1f / framesPerSecond;
            _frames = CreateFrames(spriteSheet, columns, rows);
            _spriteRenderer.sprite = _frames[0];

            if (fitToCameraWidth)
                FitToCameraWidth();

            transform.localPosition = positionOffset;
        }

        private void FitToCameraWidth()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
                throw new InvalidOperationException(name + " requires a camera tagged MainCamera.");

            float cameraHeight = mainCamera.orthographicSize * 2f;
            float cameraWidth = cameraHeight * mainCamera.aspect;
            Sprite firstFrame = _frames[0];
            float spriteWidth = firstFrame.rect.width / firstFrame.pixelsPerUnit;
            transform.localScale = Vector3.one * (cameraWidth / spriteWidth) * scaleMultiplier;
        }

        private void Update()
        {
            if (_frames.Length < 2)
                return;

            _elapsed += Time.unscaledDeltaTime;
            while (_elapsed >= _secondsPerFrame)
            {
                _elapsed -= _secondsPerFrame;
                _frameIndex = (_frameIndex + 1) % _frames.Length;
                _spriteRenderer.sprite = _frames[_frameIndex];
            }
        }

        private void OnDestroy()
        {
            foreach (Sprite frame in _frames)
            {
                if (frame != null)
                    Destroy(frame);
            }
        }

        private static Sprite[] CreateFrames(Texture2D spriteSheet, int columns, int rows)
        {
            int frameWidth = spriteSheet.width / columns;
            int frameHeight = spriteSheet.height / rows;
            var frames = new Sprite[columns * rows];

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    int frameIndex = row * columns + column;
                    int textureRow = rows - row - 1;
                    var frameRect = new Rect(
                        column * frameWidth,
                        textureRow * frameHeight,
                        frameWidth,
                        frameHeight);

                    frames[frameIndex] = Sprite.Create(
                        spriteSheet,
                        frameRect,
                        new Vector2(0.5f, 0.5f),
                        100f,
                        0,
                        SpriteMeshType.FullRect);
                    frames[frameIndex].name = spriteSheet.name + "_" + frameIndex;
                }
            }

            return frames;
        }
    }
}
