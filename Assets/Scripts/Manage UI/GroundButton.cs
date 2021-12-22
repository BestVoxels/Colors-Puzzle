using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class GroundButton : MonoBehaviour
{
    // ******GAME SHARING******
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private Image _image;

    private Texture2D _texture;
    private string _filePath = null;

    // Button Stuffs
    [SerializeField]
    private Animator _animator;
    private bool _permission = false;

    public static GroundButton Instance { get; private set; }
    private void Awake() => Instance = this;

    public void PointerDown() => _animator.Play("Press", -1, 0f);
    public void PointerExit() => _permission = false;
    public void PointerEnter() => _permission = true;
    public void PointerUp()
    {
        if (_permission)
        {
            if (GameState.IsGameStarted == false)
            {
                // Open ModePanel
                ModeUI.Instance.OpenPanel();

                // This will give sound & open/update mode info text
                ModeUI.Instance.Fetch(-1, "", "");

                // Close TypeInfo Panel & Ground Button
                StartCoroutine( TypeUI.Instance.CloseTypeInfoPanel() );
                GroundUI.Instance.ClosePanel();

                // Set Alpha
                GameState.Instance.SetPanelAlpha(80f);
                // Set StatePanel using ExtraButton script (code location not relevent to the term but just for sake)
                ExtraButton.Instance.StatePanelTo(2);

                // Save to HasClicked
                GroundUI.Instance.SaveFirstClick();

                // Set Location
                GameState.Location = "toType";
            }
            else
            {
                GameSfx.Instance.PlaySound(0);

                string best = (GamePlay.CurHighScoreBeated) ? "New Best " : "Best ";
                string mode = (GamePlay.GameMode == 3) ? "Timer Mode " : "Endless Mode ";
                int score = (GamePlay.GameMode == 3) ? GamePlay.HighScoreT : GamePlay.HighScoreE;

                // Creating sharing sentence
                string sharingText = best + "is " + score + "! " + mode + "#colorspuzzle";

                // Make it enter once...
                if (_filePath == null)
                {
                    _image.sprite = Sprite.Create(_texture, new Rect(0.0f, 0.0f, _texture.width, _texture.height), new Vector2(0.5f, 0.5f));

                    // Create Render Texture for second camera to render on
                    RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
                    rt.Create();
                    // Assign this to second camera
                    _camera.targetTexture = rt;


                    // Remember currently active render texture
                    RenderTexture currentRT = RenderTexture.active;
                    // Set the supplied RenderTexture as the active one
                    RenderTexture.active = _camera.targetTexture;

                    // Render the camera's view.
                    _camera.Render();

                    // Make a new texture and read the active Render Texture into it.
                    Texture2D ss = new Texture2D(_camera.targetTexture.width, _camera.targetTexture.height);
                    ss.ReadPixels(new Rect(0, 0, _camera.targetTexture.width, _camera.targetTexture.height), 0, 0);
                    ss.Apply();

                    // Restorie previously active render texture
                    RenderTexture.active = currentRT;

                    _filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");

                    // NativeShare's AddFile function requires you to enter the path of an existing file. File.WriteAllBytes helps us create an image file from a texture.
                    File.WriteAllBytes(_filePath, ss.EncodeToPNG());

                    // To avoid memory leaks
                    Destroy(ss);
                }

                new NativeShare().AddFile(_filePath).SetSubject("Colors Puzzle").SetText(sharingText).Share();
            }
        }

        _animator.Play(_permission ? "Up" : "Idle", -1, 0f);
    }

    // Capture screen shot first
    public IEnumerator TakeScreenShot()
    {
        // We should only readPixel with the screen with&height after End Of Frame
        yield return new WaitForEndOfFrame();

        _texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        _texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        _texture.Apply();

        // If destroy this texture the sprite image will be invisible
        // Destroy(texture);

        yield break;
    }
}