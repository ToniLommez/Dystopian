#if UNITY_EDITOR
using UnityEngine;


namespace UnityEditor
{
    [CustomEditor(typeof(AdvancedRuleTile))]
    [CanEditMultipleObjects]
    public class AdvancedRuleTileEditor : RuleTileEditor
    {
        public Texture2D AnyIcon;
        public Texture2D SpecifiedIcon;
        public Texture2D NothingIcon;

        public override void RuleOnGUI(Rect rect, Vector3Int position, int neighbor)
        {
            switch (neighbor)
            {
                case AdvancedRuleTile.Neighbor.Any:
                    UnityEngine.GUI.DrawTexture(rect, AnyIcon);
                    return;
                case AdvancedRuleTile.Neighbor.Specified:
                    UnityEngine.GUI.DrawTexture(rect, SpecifiedIcon);
                    return;
                case AdvancedRuleTile.Neighbor.Nothing:
                    UnityEngine.GUI.DrawTexture(rect, NothingIcon);
                    return;
            }

            base.RuleOnGUI(rect, position, neighbor);
        }
    }
}
#endif