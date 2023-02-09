using UnityEngine;

public class CursorManager : Singleton<CursorManager>
{
    Texture2D cardCursor;
    Texture2D original;
    Texture2D cancelCardCursor;

    void Start()
    {
        cardCursor = Resources.Load<Texture2D>("CardCursor");
        original = Resources.Load<Texture2D>("Original");
        cancelCardCursor = Resources.Load<Texture2D>("CancelCardCursor");
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
        
        if (!CardManager.GetInstance.ableAddCard)
        {
            Cursor.SetCursor(cancelCardCursor, new Vector2(0, 0), CursorMode.ForceSoftware);
        }
    }
}
