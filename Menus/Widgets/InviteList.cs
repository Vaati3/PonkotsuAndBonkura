using Godot;
using GodotSteam;

public partial class InviteList : Panel
{
    private partial class InviteButton : Button
    {
        ulong friendId;

        public delegate void CloseListCallback();
        CloseListCallback CloseList;

        public InviteButton(ulong friendId, CloseListCallback closeList)
        {
            this.friendId = (ulong)friendId;
            CloseList += closeList;
            Pressed += _on_button_pressed;
            Text = Steam.GetFriendPersonaName(this.friendId);

    		Theme = GD.Load<Theme>("res://Menus/Themes/Button.tres");
            CustomMinimumSize = new Vector2(100, 50);
        }

        public void _on_button_pressed()
        {
            Steam.InviteUserToGame(friendId, "test");
            CloseList();
        }
    }

    public override void _Ready()
    {
        GridContainer grid = GetNode<GridContainer>("Grid");
        int n = Steam.GetFriendCount();
        for (int i = 0; i < n; i++)
        {
            ulong friendId = Steam.GetFriendByIndex(i, FriendFlag.Immediate);
            if (Steam.GetFriendPersonaState(friendId) != PersonaState.Offline)
                grid.AddChild(new InviteButton(friendId, Close));
        }
    }

    public void Close()
    {
        QueueFree();
    }
}