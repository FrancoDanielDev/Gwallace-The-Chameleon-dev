using UnityEngine;

public class PlayerInteractions
{
    private Player _player;

    public PlayerInteractions(Player player)
    {
        _player = player;
    }

    public void OnTriggerEnter(Collider other)
    {
        FallingPin platform = other.GetComponent<FallingPin>();
        if (platform != null) FallingPlatform(platform);

        Bubble bubble = other.GetComponent<Bubble>();
        if (bubble != null) _player.FormSwitch.BubblePerformance(bubble);

        JumpPad jumpPad = other.GetComponent<JumpPad>();
        if (jumpPad != null) jumpPad.Bounce(_player.gameObject);

        CrashBox crashBox = other.GetComponent<CrashBox>();
        if (crashBox != null) crashBox.DoAction(_player.gameObject);

        Leaf leaf = other.GetComponent<Leaf>();
        if (leaf != null)
        {
            var gnawedLeaf = leaf.GetComponent<GnawedLeaf>();
            var ultimateLeaf = leaf.GetComponent<UltimateLeaf>();

            if (gnawedLeaf != null) _player.PlayerTechs.EvaluteGoingNextLevel();
            else if (ultimateLeaf != null) _player.PlayerTechs.EndVoyage();

            leaf.ConsumeLeaf();
        }

        if (!_player.Immortal && (other.GetComponent<KillPlane>() || other.CompareTag("Enemy")))
            _player.PlayerTechs.DieAndSpawn();

        StartingLine startingLine = other.GetComponent<StartingLine>();
        if (startingLine != null) startingLine.StartSpeedrun();
    }

    private void FallingPlatform(FallingPin platform)
    {
        switch (_player.CurrentForm)
        {
            case FormName.Frog:
                Debug.Log("Cuz I'm a Frog I can walk around here.");
                break;

            default:
                platform.MakeItFall();
                break;
        }
    }
}
