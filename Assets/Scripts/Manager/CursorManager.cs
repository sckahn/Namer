using UnityEngine;

public class CursorManager : Singleton<CursorManager>
{
    Texture2D cardCursor;
    Texture2D original;

    void Start()
    {
        cardCursor = Resources.Load<Texture2D>("CardCursor");
        original = Resources.Load<Texture2D>("Original");
    }

    private void Update()
    {
        if (CardManager.GetInstance.isPickCard)
        {
            Cursor.SetCursor(cardCursor, new Vector2(0, 0), CursorMode.ForceSoftware);
        }
        else
        {
            Cursor.SetCursor(original, new Vector2(0, 0), CursorMode.ForceSoftware);
        }
    }
}
